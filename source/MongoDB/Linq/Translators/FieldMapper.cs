using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq.Translators
{
    internal class FieldMapper : MongoExpressionVisitor
    {
        private HashSet<string> _oldAliases;
        private string _newAlias;

        public Expression Map(Expression expression, string newAlias, IEnumerable<string> oldAliases)
        {
            _oldAliases = new HashSet<string>(oldAliases);
            _newAlias = newAlias;
            return Visit(expression);
        }

        public Expression Map(Expression expression, string newAlias, params string[] oldAliases)
        {
            return Map(expression, newAlias, (IEnumerable<string>)oldAliases);
        }

        protected override Expression VisitField(FieldExpression field)
        {
            if (_oldAliases.Contains(field.Alias))
                return new FieldExpression(field.Expression, _newAlias, field.Name);
            return field;
        }
    }
}