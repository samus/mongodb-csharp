using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Driver.Linq.Expressions;

namespace MongoDB.Driver.Linq
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

        //protected override Expression VisitProjection(ProjectionExpression p)
        //{
        //    var subQuery = Expression.Lambda(base.VisitProjection(p), _document);
        //    var elementType = TypeSystem.GetElementType(subQuery.Body.Type);
        //    var mi = typeof(ProjectionBuilder)
        //        .GetMethod("ExecuteSubQuery", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
        //        .MakeGenericMethod(elementType);

        //    return Expression.Convert(
        //        Expression.Call(mi, Expression.Constant(subQuery)),
        //        p.Type);
        //}

        //private static IEnumerable<T> ExecuteSubQuery<T>(LambdaExpression subQuery)
        //{
        //    //var projection = (ProjectionExpression)new Replacer().Replace(subQuery.Body, subQuery.Parameters[0], Expression.Constant(this));
        //    var projection = (ProjectionExpression)PartialEvaluator.Evaluate(subQuery.Body, CanBeEvaluatedLocally);
        //    return null;
        //}

        //private static bool CanBeEvaluatedLocally(Expression expression)
        //{
        //    if(expression.NodeType == ExpressionType.Parameter)
        //        return false;

        //    return true;
        //}
    }
}