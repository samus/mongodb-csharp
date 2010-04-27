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
        private string _groupByAlias;

        public Expression AggregateInGroupSelect
        {
            get { return _aggregateInGroupSelect; }
        }

        public ScalarExpression AggregateAsSubquery
        {
            get { return _aggregateAsSubquery; }
        }

        public string GroupByAlias
        {
            get { return _groupByAlias; }
        }

        public AggregateSubqueryExpression(string groupByAlias, Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery)
            : base((ExpressionType)MongoExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            _groupByAlias = groupByAlias;
            _aggregateInGroupSelect = aggregateInGroupSelect;
            _aggregateAsSubquery = aggregateAsSubquery;
        }
    }
}