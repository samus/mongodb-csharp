using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Linq.Expressions
{
    internal class NamedValueExpression : MongoExpression
    {
        private readonly string _name;
        private readonly Expression _value;

        public string Name
        {
            get { return _name; }
        }
        public Expression Value
        {
            get { return _value; }
        }

        public NamedValueExpression(string name, Expression value)
            : base(MongoExpressionType.NamedValue, value.Type)
        {
            _name = name;
            _value = value;
        }
        
    }
}
