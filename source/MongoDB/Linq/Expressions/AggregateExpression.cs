using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Linq.Expressions
{
    internal class AggregateExpression : MongoExpression
    {
        private readonly AggregateType _aggregateType;
        private readonly Expression _argument;
        private readonly bool _distinct;

        public AggregateType AggregateType
        {
            get { return _aggregateType; }
        }

        public Expression Argument
        {
            get { return _argument; }
        }

        public bool Distinct
        {
            get { return _distinct; }
        }

        public AggregateExpression(Type type, AggregateType aggregateType, Expression argument, bool distinct)
            : base(MongoExpressionType.Aggregate, type)
        {
            _aggregateType = aggregateType;
            _argument = argument;
            _distinct = distinct;
        }
    }
}
