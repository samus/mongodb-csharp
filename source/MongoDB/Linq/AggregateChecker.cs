using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq
{
    internal class AggregateChecker : MongoExpressionVisitor
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
}
