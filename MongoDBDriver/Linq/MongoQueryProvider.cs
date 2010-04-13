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
            return ExecuteQueryObject(queryObject);
        }

        internal object ExecuteQueryObject(MongoQueryObject queryObject)
        {
            var miGetCollection = typeof(IMongoDatabase).GetMethods().Where(m => m.Name == "GetCollection" && m.GetGenericArguments().Length == 1).Single().MakeGenericMethod(queryObject.DocumentType);

            var collection = miGetCollection.Invoke(queryObject.Database, new [] { queryObject.CollectionName });

            var cursor = collection.GetType().GetMethod("FindAll")
                            .Invoke(collection, null);
            var cursorType = cursor.GetType();
            cursorType.GetMethod("Spec", new[] { typeof(Document) }).Invoke(cursor, new object[] { queryObject.Query });
            cursorType.GetMethod("Fields", new[] { typeof(Document) }).Invoke(cursor, new object[] { queryObject.Fields });
            cursorType.GetMethod("Limit").Invoke(cursor, new object[] { queryObject.NumberToLimit });
            cursorType.GetMethod("Skip").Invoke(cursor, new object[] { queryObject.NumberToSkip });

            var projector = queryObject.Projector.Compile();
            return Activator.CreateInstance(
                typeof(ProjectionReader<,>).MakeGenericType(queryObject.DocumentType, projector.Method.ReturnType),
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new object[] { cursor, projector },
                null);
        }

        internal MongoQueryObject GetQueryObject(Expression expression)
        {
            expression = PartialEvaluator.Evaluate(expression, CanBeEvaluatedLocally);
            var projection = (ProjectionExpression)new QueryBinder().Bind(expression);
            var queryObject = new QueryFormatter().Format(projection.Source);
            queryObject.Projector = new ProjectionBuilder().Build(queryObject.DocumentType, projection.Projector);
            return queryObject;
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter &&
                   expression.NodeType != ExpressionType.Lambda;
        }
    }
}