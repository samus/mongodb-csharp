using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Linq
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
                var fromFind = (FindExpression)f.From;

                f = (FindExpression)new FindRemover().Remove(f, new[] { fromFind });
                   
                var where = f.Where;
                if(fromFind.Where != null)
                {
                    if (where != null)
                        where = Expression.And(fromFind.Where, where);
                    else
                        where = fromFind.Where;
                }

                var groupBy = f.GroupBy != null && f.GroupBy.Count > 0 ? f.GroupBy : fromFind.GroupBy;
                var orderBy = f.OrderBy != null && f.OrderBy.Count > 0 ? f.OrderBy : fromFind.OrderBy;
                var skip = f.Skip != null ? f.Skip : fromFind.Skip;
                var limit = f.Limit != null ? f.Limit : fromFind.Limit;
                bool distinct = f.Distinct | fromFind.Distinct;
                var fields = f.Fields.Count > 0 ? f.Fields : fromFind.Fields;

                if (where != f.Where
                    || orderBy != f.OrderBy
                    || groupBy != f.GroupBy
                    || distinct != f.Distinct
                    || skip != f.Skip
                    || limit != f.Limit
                    || fields != f.Fields)
                {
                    f = new FindExpression(f.Type, f.Alias, fields, f.From, where, orderBy, groupBy, distinct, skip, limit);
                }
            }

            return f;
        }
        
        private static bool CanMergeWithFrom(FindExpression find, bool isTopLevel)
        {
            var fromFind = find.From as FindExpression;
            if (fromFind == null)
                return false;

            var fromIsSimpleProjection = IsSimpleProjection(fromFind);
            var fromIsNameMapProjection = IsNameMapProjection(fromFind);
            if (!fromIsSimpleProjection && !fromIsNameMapProjection)
                return false;

            var findIsNameMapProjection = IsNameMapProjection(find);
            var findHasOrderBy = find.OrderBy != null && find.OrderBy.Count > 0;
            var findHasGroupBy = find.GroupBy != null && find.GroupBy.Count > 0;
            var findHasAggregates = new AggregateChecker().HasAggregates(find);
            var fromHasOrderBy = fromFind.OrderBy != null && fromFind.OrderBy.Count > 0;
            var fromHasGroupBy = fromFind.GroupBy != null && fromFind.OrderBy.Count > 0;

            if (findHasOrderBy && fromHasOrderBy)
                return false;

            if (findHasGroupBy && fromHasGroupBy)
                return false;

            if(fromHasOrderBy && (findHasGroupBy || findHasAggregates || find.Distinct))
                return false;

            if(fromHasGroupBy && find.Where != null)
                return false;

            if(fromFind.Limit != null && (find.Limit != null || find.Skip != null || find.Distinct || findHasAggregates || findHasGroupBy))
                return false;

            if(fromFind.Skip != null && (find.Skip != null || find.Distinct || findHasAggregates || findHasGroupBy))
                return false;

            if (fromFind.Distinct && (find.Limit != null || find.Skip != null || !findIsNameMapProjection || findHasGroupBy || findHasAggregates || (findHasOrderBy && !isTopLevel)))
                return false;

            return true;
        }

        private static bool IsNameMapProjection(FindExpression find)
        {
            var fromFind = find.From as FindExpression;
            if (find.Fields.Count == 0)
                return true;

            if (fromFind == null || find.Fields.Count != fromFind.Fields.Count)
                return false;

            for (int i = 0, n = find.Fields.Count; i < n; i++)
            {
                if (find.Fields[i].Name != fromFind.Fields[i].Name)
                    return false;
            }

            return true;
        }

        private static bool IsSimpleProjection(FindExpression find)
        {
            foreach (var field in find.Fields)
                if (string.IsNullOrEmpty(field.Name))
                    return false;

            return true;
        }

        private class AggregateChecker : MongoExpressionVisitor
        {
            private bool _hasAggregate;

            public AggregateChecker()
            { }

            public bool HasAggregates(Expression expression)
            {
                _hasAggregate = false;
                Visit(expression);
                return _hasAggregate;
            }

            protected override Expression VisitAggregate(AggregateExpression aggregate)
            {
                _hasAggregate = true;
                return aggregate;
            }

            protected override Expression VisitFind(FindExpression find)
            {
                Visit(find.Where);
                VisitOrderBy(find.OrderBy);
                VisitFieldDeclarationList(find.Fields);
                return find;
            }

            protected override Expression VisitSubquery(SubqueryExpression subquery)
            {
                return subquery;
            }
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
    }
}