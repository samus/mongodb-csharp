using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq.Translators
{
    internal class ExpressionReplacer : MongoExpressionVisitor
    {
        private Expression _replaceWith;
        private Expression _searchFor;

        public Expression Replace(Expression expression, Expression searchFor, Expression replaceWith)
        {
            this._searchFor = searchFor;
            this._replaceWith = replaceWith;
            return Visit(expression);
        }

        public Expression ReplaceAll(Expression expression, Expression[] searchFor, Expression[] replaceWith)
        {
            for (int i = 0, n = searchFor.Length; i < n; i++)
                expression = Replace(expression, searchFor[i], replaceWith[i]);
            return expression;
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == _searchFor)
                return _replaceWith;
            return base.Visit(exp);
        }

    }
}
