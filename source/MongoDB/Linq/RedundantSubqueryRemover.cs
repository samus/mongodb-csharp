using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Linq
{
    internal class RedundantSubqueryRemover : MongoExpressionVisitor
    {
        private bool _isTopLevel;

        public Expression Merge(Expression expression)
        {
            _isTopLevel = true;
            return Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            bool wasTopLevel = _isTopLevel;
            _isTopLevel = false;

            select = (SelectExpression)base.VisitSelect(select);

            while (CanMergeWithFrom(select, wasTopLevel))
            {
                var fromSelect = (SelectExpression)select.From;

                select = (SelectExpression)new SelectRemover().Remove(select, new[] { fromSelect });
                   
                var where = select.Where;
                if(fromSelect.Where != null)
                {
                    if (where != null)
                        where = Expression.And(fromSelect.Where, where);
                    else
                        where = fromSelect.Where;
                }

                var groupBy = select.GroupBy != null ? select.GroupBy : fromSelect.GroupBy;
                var orderBy = select.OrderBy != null && select.OrderBy.Count > 0 ? select.OrderBy : fromSelect.OrderBy;
                var skip = select.Skip != null ? select.Skip : fromSelect.Skip;
                var limit = select.Limit != null ? select.Limit : fromSelect.Limit;
                bool distinct = select.Distinct | fromSelect.Distinct;
                var fields = select.Fields.Count > 0 ? select.Fields : fromSelect.Fields;

                if (where != select.Where
                    || orderBy != select.OrderBy
                    || groupBy != select.GroupBy
                    || distinct != select.Distinct
                    || skip != select.Skip
                    || limit != select.Limit
                    || fields != select.Fields)
                {
                    select = new SelectExpression(select.Type, select.Alias, fields, select.From, where, orderBy, groupBy, distinct, skip, limit);
                }
            }

            return select;
        }
        
        private static bool CanMergeWithFrom(SelectExpression select, bool isTopLevel)
        {
            var fromSelect = select.From as SelectExpression;
            if (fromSelect == null)
                return false;

            var fromIsSimpleProjection = IsSimpleProjection(fromSelect);
            var fromIsNameMapProjection = IsNameMapProjection(fromSelect);
            if (!fromIsSimpleProjection && !fromIsNameMapProjection)
                return false;

            var selectIsNameMapProjection = IsNameMapProjection(select);
            var selectHasOrderBy = select.OrderBy != null && select.OrderBy.Count > 0;
            var selectHasGroupBy = select.GroupBy != null;
            var selectHasAggregates = new AggregateChecker().HasAggregates(select);
            var fromHasOrderBy = fromSelect.OrderBy != null && fromSelect.OrderBy.Count > 0;
            var fromHasGroupBy = fromSelect.GroupBy != null;

            if (selectHasOrderBy && fromHasOrderBy)
                return false;

            if (selectHasGroupBy && fromHasGroupBy)
                return false;

            if(fromHasOrderBy && (selectHasGroupBy || selectHasAggregates || select.Distinct))
                return false;

            if(fromHasGroupBy && select.Where != null)
                return false;

            if(fromSelect.Limit != null && (select.Limit != null || select.Skip != null || select.Distinct || selectHasAggregates || selectHasGroupBy))
                return false;

            if(fromSelect.Skip != null && (select.Skip != null || select.Distinct || selectHasAggregates || selectHasGroupBy))
                return false;

            if (fromSelect.Distinct && (select.Limit != null || select.Skip != null || !selectIsNameMapProjection || selectHasGroupBy || selectHasAggregates || (selectHasOrderBy && !isTopLevel)))
                return false;

            return true;
        }

        private static bool IsNameMapProjection(SelectExpression select)
        {
            var fromSelect = select.From as SelectExpression;
            if (select.Fields.Count == 0)
                return true;

            if (fromSelect == null || select.Fields.Count != fromSelect.Fields.Count)
                return false;

            for (int i = 0, n = select.Fields.Count; i < n; i++)
            {
                if (select.Fields[i].Name != fromSelect.Fields[i].Name)
                    return false;
            }

            return true;
        }

        private static bool IsSimpleProjection(SelectExpression select)
        {
            foreach (var field in select.Fields)
                if (string.IsNullOrEmpty(field.Name))
                    return false;

            return true;
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
    }
}