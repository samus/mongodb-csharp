using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;

namespace MongoDB.Linq
{
    internal class ProjectionBuilder : MongoExpressionVisitor
    {
        private ParameterExpression _document;

        public LambdaExpression Build(Type documentType, Expression expression)
        {
            _document = Expression.Parameter(documentType, "document");
            var body = Visit(expression);
            return Expression.Lambda(body, _document);
        }

        protected override Expression VisitField(FieldExpression f)
        {
            return Visit(f.Expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p.Type == _document.Type)
                return _document;

            return p;
        }

        protected override Expression VisitProjection(ProjectionExpression p)
        {
            throw new InvalidQueryException("Nested queries are not currently supported.");
        }
    }
}