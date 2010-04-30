using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Linq.Expressions
{
    internal class AggregateSubqueryExpression : MongoExpression
    {
        private Expression _aggregateInGroupSelect;
        private ScalarExpression _aggregateAsSubquery;
        private Alias _groupByAlias;

        public Expression AggregateInGroupSelect
        {
            get { return _aggregateInGroupSelect; }
        }

        public ScalarExpression AggregateAsSubquery
        {
            get { return _aggregateAsSubquery; }
        }

        public Alias GroupByAlias
        {
            get { return _groupByAlias; }
        }

        public AggregateSubqueryExpression(Alias groupByAlias, Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery)
            : base(MongoExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            _groupByAlias = groupByAlias;
            _aggregateInGroupSelect = aggregateInGroupSelect;
            _aggregateAsSubquery = aggregateAsSubquery;
        }
    }
}