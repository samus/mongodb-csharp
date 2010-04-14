using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class SelectExpression : Expression
    {
        private readonly ReadOnlyCollection<string> _fields;
        private readonly Expression _from;
        private readonly Expression _where;

        public ReadOnlyCollection<string> Fields
        {
            get { return _fields; }
        }

        public Expression From
        {
            get { return _from; }
        }

        public Expression Where
        {
            get { return _where; }
        }

        public SelectExpression(Type type, IEnumerable<string> fields, Expression from, Expression where)
            : base((ExpressionType)MongoExpressionType.Select, type)
        {
            _fields = fields as ReadOnlyCollection<string>;
            if (_fields == null)
                _fields = new List<string>(fields).AsReadOnly();

            _from = from;
            _where = where;
        }
    }
}
