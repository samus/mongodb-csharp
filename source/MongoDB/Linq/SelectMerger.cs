using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Driver.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Driver.Linq
{
    internal class SelectMerger : MongoExpressionVisitor
    {
        private bool _isTopLevel;

        public Expression Merge(Expression expression)
        {
            _isTopLevel = true;
            return Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression s)
        {
            bool wasTopLevel = _isTopLevel;
            _isTopLevel = false;

            s = (SelectExpression)base.VisitSelect(s);

            while (CanMergeWithFrom(s, wasTopLevel))
            {
                var fromSelect = (SelectExpression)s.From;

                s = (SelectExpression)new SelectRemover().Remove(s, new[] { fromSelect });

                var oldFields = s.Fields.Count > 0 ? s.Fields : fromSelect.Fields;

                var fields = new List<FieldExpression>();
                foreach(var field in oldFields)
                    fields.AddRange(new FieldExpander().Expand(field));
                    
                var where = s.Where;
                if(fromSelect.Where != null)
                {
                    if (where != null)
                        where = Expression.And(fromSelect.Where, where);
                    else
                        where = fromSelect.Where;
                }

                var order = s.Order != null && s.Order.Count > 0 ? s.Order : fromSelect.Order;
                var skip = s.Skip != null ? s.Skip : fromSelect.Skip;
                var limit = s.Limit != null ? s.Limit : fromSelect.Limit;
                bool distinct = s.Distinct | fromSelect.Distinct;
                

                if (where != s.Where
                    || order != s.Order
                    || distinct != s.Distinct
                    || skip != s.Skip
                    || limit != s.Limit)
                {
                    s = new SelectExpression(s.Type, fields, s.From, where, order, distinct, skip, limit);
                }
            }

            return s;
        }
        
        private static bool CanMergeWithFrom(SelectExpression select, bool isTopLevel)
        {
            return select.From is SelectExpression;
        }

        private class SelectRemover : MongoExpressionVisitor
        {
            private HashSet<SelectExpression> _selectsToRemove;

            public Expression Remove(SelectExpression outerSelect, IEnumerable<SelectExpression> selectsToRemove)
            {
                _selectsToRemove = new HashSet<SelectExpression>(selectsToRemove);
                return Visit(outerSelect);
            }

            protected override Expression VisitSelect(SelectExpression s)
            {
                if (_selectsToRemove.Contains(s))
                    return Visit(s.From);

                return base.VisitSelect(s);
            }
        }

        private class FieldExpander : MongoExpressionVisitor
        {
            private FieldExpression _root;
            private List<FieldExpression> _map;

            public ReadOnlyCollection<FieldExpression> Expand(FieldExpression field)
            {
                _map = new List<FieldExpression>();
                _root = field;
                VisitField(field);
                if (_map.Count == 0)
                    _map.Add(field);

                return _map.AsReadOnly();
            }

            protected override Expression VisitField(FieldExpression f)
            {
                if (_root != f)
                {
                    var fields = new FieldExpander().Expand(f);
                    _map.AddRange(fields);
                }

                return base.VisitField(f);
            }
        }
    }
}