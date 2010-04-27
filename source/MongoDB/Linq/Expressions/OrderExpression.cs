using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class OrderExpression : Expression
    {
        private readonly Expression _expression;
        private readonly OrderType _orderType;

        public Expression Expression
        {
            get { return _expression; }
        }

        public OrderType OrderType
        {
            get { return _orderType; }
        }

        public OrderExpression(OrderType orderType, Expression expression)
            : base((ExpressionType)MongoExpressionType.Order, expression.Type)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _expression = expression;
            _orderType = orderType;
        }
    }
}