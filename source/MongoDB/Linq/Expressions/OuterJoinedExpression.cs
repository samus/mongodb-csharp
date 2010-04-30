using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal class OuterJoinedExpression : MongoExpression
    {
        private readonly Expression _expression;
        private readonly Expression _test;

        public Expression Expression
        {
            get { return _expression; }
        }

        public Expression Test
        {
            get { return _test; }
        }

        public OuterJoinedExpression(Expression test, Expression expression)
            : base(MongoExpressionType.OuterJoined, expression.Type)
        {
            _test = test;
            _expression = expression;
        }
    }
}