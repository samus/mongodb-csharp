using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Linq.Expressions;
using System.Collections.ObjectModel;

namespace MongoDB.Linq.Translators
{
    internal class ProjectionBuilder : MongoExpressionVisitor
    {
        private MongoQueryObject _queryObject;
        private ParameterExpression _document;
        private GroupingKeyDeterminer _determiner;

        public ProjectionBuilder()
        {
            _determiner = new GroupingKeyDeterminer();
        }

        public LambdaExpression Build(MongoQueryObject queryObject, Expression projector)
        {
            _queryObject = queryObject;
            if (_queryObject.IsMapReduce)
                _document = Expression.Parameter(typeof(Document), "document");
            else
                _document = Expression.Parameter(queryObject.DocumentType, "document");

            var body = Visit(projector);
            return Expression.Lambda(body, _document);
        }

        protected override Expression VisitField(FieldExpression field)
        {
            if (_queryObject.IsMapReduce)
            {
                var parts = field.Name.Split('.');

                bool isGroupingField = _determiner.IsGroupingKey(field);
                Expression current;
                if(parts.Contains("Key") && isGroupingField)
                    current = _document;
                else
                {
                    current = Expression.Call(
                        _document,
                        "Get",
                        new [] { typeof(Document) },
                        Expression.Constant("value"));
                }

                for (int i = 0, n = parts.Length; i < n; i++)
                {
                    var type = i == n - 1 ? field.Type : typeof(Document);

                    if (parts[i] == "Key" && isGroupingField)
                        parts[i] = "_id";

                    current = Expression.Call(
                        current,
                        "Get",
                        new[] { type },
                        Expression.Constant(parts[i]));
                }

                return current;
            }
            else
                return Visit(field.Expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p.Type == _document.Type)
                return _document;

            return p;
        }

        private class GroupingKeyDeterminer : MongoExpressionVisitor
        {
            private bool _isGroupingKey;

            public bool IsGroupingKey(Expression exp)
            {
                _isGroupingKey = false;
                Visit(exp);
                return _isGroupingKey;
            }

            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                    return exp;

                if (_isGroupingKey)
                    return exp;

                if (exp.Type.IsGenericType && exp.Type.GetGenericTypeDefinition() == typeof(Grouping<,>))
                {
                    _isGroupingKey = true;
                    return exp;
                }
                return base.Visit(exp);
            }
        }
    }
}