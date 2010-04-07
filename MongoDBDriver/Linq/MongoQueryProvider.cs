using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MongoDB.Driver.Connections;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Linq
{
    public class MongoQueryProvider : IQueryProvider
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _database;

        public string CollectionName
        {
            get { return _collectionName; }
        }

        public IMongoDatabase Database
        {
            get { return _database; }
        }

        public MongoQueryProvider(IMongoDatabase database, string collectionName)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (collectionName == null)
                throw new ArgumentNullException("collectionName");

            _collectionName = collectionName;
            _database = database;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MongoQuery<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(MongoQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public object Execute(Expression expression)
        {
            var queryObject = GetQueryObject(expression);

            throw new NotImplementedException();
            //var cursor = _collection.GetType().GetMethod("FindAll")
            //                .Invoke(_collection, null);
            //var cursorType = cursor.GetType();
            //cursorType.GetMethod("Spec", new [] { typeof(Document) }).Invoke(cursor, new object[] { queryObject.Query });
            //if(queryObject.Projection != null)
            //    cursorType.GetMethod("Fields", new [] { typeof(Document) }).Invoke(cursor, new object[] { queryObject.Projection.CreateDocument() });
            //cursorType.GetMethod("Limit").Invoke(cursor, new object[] { queryObject.NumberToLimit });
            //cursorType.GetMethod("Skip").Invoke(cursor, new object[] { queryObject.NumberToSkip });

            //Type elementType = TypeSystem.GetElementType(expression.Type);
            //if (queryObject.Projection != null)
            //{
            //    var projector = queryObject.Projection.Projector.Compile();
            //    return Activator.CreateInstance(
            //        typeof(ProjectionReader<,>).MakeGenericType(cursorType.GetGenericArguments()[0], elementType),
            //        BindingFlags.Instance | BindingFlags.Public,
            //        null,
            //        new object[] { cursor, projector },
            //        null);
            //}

            //return cursor.GetType().GetProperty("Documents").GetValue(cursor, null);
        }

        internal MongoQueryObject GetQueryObject(Expression expression)
        {
            expression = PartialEvaluator.Evaluate(expression, CanBeEvaluatedLocally);
            var projection = (ProjectionExpression)new QueryBinder().Bind(expression);
            var queryObject = new QueryFormatter().Format(projection.Source);
            queryObject.Projector = Expression.Lambda(projection.Projector, Expression.Parameter(queryObject.DocumentType, "document"));
            return queryObject;
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter &&
                   expression.NodeType != ExpressionType.Lambda;
        }
    }
}