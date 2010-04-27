using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Linq.Expressions
{
    internal class Replacer : MongoExpressionVisitor
    {
        private Expression _replaceWith;
        private Expression _searchFor;

        internal Expression Replace(Expression expression, Expression searchFor, Expression replaceWith)
        {
            _searchFor = searchFor;
            _replaceWith = replaceWith;

            return Visit(expression);
        }
        protected override Expression Visit(Expression exp)
        {
            if (exp == _searchFor)
            {
                return _replaceWith;
            }
            return base.Visit(exp);
        }
    }
}
