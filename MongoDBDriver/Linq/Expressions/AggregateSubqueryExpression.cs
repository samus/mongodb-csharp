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
        private string _alias;

        public Expression AggregateInGroupSelect
        {
            get { return _aggregateInGroupSelect; }
        }

        public ScalarExpression AggregateAsSubquery
        {
            get { return _aggregateAsSubquery; }
        }

        public string Alias
        {
            get { return _alias; }
        }

        public AggregateSubqueryExpression(string alias, Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery)
            : base((ExpressionType)MongoExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            _alias = alias;
            _aggregateInGroupSelect = aggregateInGroupSelect;
            _aggregateAsSubquery = aggregateAsSubquery;
        }
    }
}