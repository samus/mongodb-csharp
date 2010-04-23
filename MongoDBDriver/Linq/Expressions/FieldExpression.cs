using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class FieldExpression : Expression
    {
        private readonly Expression _expression;
        private readonly string _name;

        public Expression Expression
        {
            get { return _expression; }
        }

        public string Name
        {
            get { return _name; }
        }

        public FieldExpression(string name, Expression expression)
            : base((ExpressionType)MongoExpressionType.Field, expression.Type)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (expression == null)
                throw new ArgumentNullException("expression");

            _expression = expression;
            _name = name;
        }
    }
}
