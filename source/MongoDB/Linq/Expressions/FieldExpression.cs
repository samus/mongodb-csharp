using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal class FieldExpression : MongoExpression
    {
        private readonly string _alias;
        private readonly Expression _expression;
        private readonly string _name;

        public string Alias
        {
            get { return _alias; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public string Name
        {
            get { return _name; }
        }

        public FieldExpression(Expression expression, string alias, string name)
            : base(MongoExpressionType.Field, expression.Type)
        {
            _alias = alias;
            _expression = expression;
            _name = name;
        }
    }
}