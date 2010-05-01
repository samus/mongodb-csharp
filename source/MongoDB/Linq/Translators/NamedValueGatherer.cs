using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq.Translators
{
    internal class NamedValueGatherer : MongoExpressionVisitor
    {
        private List<NamedValueExpression> _namedValues;

        public ReadOnlyCollection<NamedValueExpression> Gather(Expression expr)
        {
            _namedValues = new List<NamedValueExpression>();
            Visit(expr);
            return _namedValues.AsReadOnly();
        }

        protected override Expression VisitNamedValue(NamedValueExpression value)
        {
            _namedValues.Add(value);
            return value;
        }
    }
}
