using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class AggregateSubqueryExpression : Expression
    {
        private Expression _aggregateInGroupSelect;
        private ScalarExpression _aggregateAsSubquery;

        public Expression AggregateInGroupSelect
        {
            get { return _aggregateInGroupSelect; }
        }

        public ScalarExpression AggregateAsSubquery
        {
            get { return _aggregateAsSubquery; }
        }

        public AggregateSubqueryExpression(Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery)
            : base((ExpressionType)MongoExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            _aggregateInGroupSelect = aggregateInGroupSelect;
            _aggregateAsSubquery = aggregateAsSubquery;
        }
    }
}
