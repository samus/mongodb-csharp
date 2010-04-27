using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Driver.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Driver.Linq
{
    internal class FindMerger : MongoExpressionVisitor
    {
        private bool _isTopLevel;

        public Expression Merge(Expression expression)
        {
            _isTopLevel = true;
            return Visit(expression);
        }

        protected override Expression VisitFind(FindExpression f)
        {
            bool wasTopLevel = _isTopLevel;
            _isTopLevel = false;

            f = (FindExpression)base.VisitFind(f);

            while (CanMergeWithFrom(f, wasTopLevel))
            {
                var fromSelect = (FindExpression)f.From;

                f = (FindExpression)new FindRemover().Remove(f, new[] { fromSelect });

                var oldFields = f.Fields.Count > 0 ? f.Fields : fromSelect.Fields;

                var fields = new List<FieldExpression>();
                foreach(var field in oldFields)
                    fields.AddRange(new FieldExpander().Expand(field));
                    
                var where = f.Where;
                if(fromSelect.Where != null)
                {
                    if (where != null)
                        where = Expression.And(fromSelect.Where, where);
                    else
                        where = fromSelect.Where;
                }

                var groupBy = f.GroupBy != null && f.GroupBy.Count > 0 ? f.GroupBy : fromSelect.GroupBy;
                var orderBy = f.OrderBy != null && f.OrderBy.Count > 0 ? f.OrderBy : fromSelect.OrderBy;
                var skip = f.Skip != null ? f.Skip : fromSelect.Skip;
                var limit = f.Limit != null ? f.Limit : fromSelect.Limit;
                bool distinct = f.Distinct | fromSelect.Distinct;
                

                if (where != f.Where
                    || groupBy != f.GroupBy
                    || orderBy != f.OrderBy
                    || distinct != f.Distinct
                    || skip != f.Skip
                    || limit != f.Limit)
                {
                    f = new FindExpression(f.Type, fields, f.From, where, orderBy, groupBy, distinct, skip, limit);
                }
            }

            return f;
        }
        
        private static bool CanMergeWithFrom(FindExpression select, bool isTopLevel)
        {
            var fromSelect = select.From as FindExpression;
            if (fromSelect == null)
                return false;

            if (select.Limit != null && fromSelect.Limit != null)
                return false;

            if (select.Skip != null && fromSelect.Skip != null)
                return false;

            if (select.GroupBy != null && select.GroupBy.Count > 0 && fromSelect.GroupBy != null && fromSelect.GroupBy.Count > 0)
                return false;

            return true;
        }

        private class FindRemover : MongoExpressionVisitor
        {
            private HashSet<FindExpression> _selectsToRemove;

            public Expression Remove(FindExpression outerSelect, IEnumerable<FindExpression> selectsToRemove)
            {
                _selectsToRemove = new HashSet<FindExpression>(selectsToRemove);
                return Visit(outerSelect);
            }

            protected override Expression VisitFind(FindExpression s)
            {
                if (_selectsToRemove.Contains(s))
                    return Visit(s.From);

                return base.VisitFind(s);
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

            protected override Expression VisitAggregate(AggregateExpression a)
            {
                return a;
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