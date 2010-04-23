using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Driver.Connections;
using MongoDB.Driver.Linq.Expressions;
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
            if (queryObject.IsCount)
                return ExecuteCount(queryObject);
            else
                return ExecuteFind(queryObject);
        }

        internal MongoQueryObject GetQueryObject(Expression expression)
        {
            expression = PartialEvaluator.Evaluate(expression, CanBeEvaluatedLocally);
            expression = new FieldBinder().Bind(expression);
            var projection = (ProjectionExpression)new QueryBinder(this, expression).Bind(expression);
            var queryObject = new QueryFormatter().Format(projection.Source);
            queryObject.Projector = new ProjectionBuilder().Build(queryObject.DocumentType, projection.Projector);
            queryObject.Aggregator = projection.Aggregator;
            return queryObject;
        }

        private bool CanBeEvaluatedLocally(Expression expression)
        {
            // any operation on a query can't be done locally
            ConstantExpression cex = expression as ConstantExpression;
            if (cex != null)
            {
                IQueryable query = cex.Value as IQueryable;
                if (query != null && query.Provider == this)
                    return false;
            }
            MethodCallExpression mc = expression as MethodCallExpression;
            if (mc != null && (mc.Method.DeclaringType == typeof(Enumerable) || mc.Method.DeclaringType == typeof(Queryable) || mc.Method.DeclaringType == typeof(MongoQueryable)))
            {
                return false;
            }
            if (expression.NodeType == ExpressionType.Convert &&
                expression.Type == typeof(object))
                return true;
            return expression.NodeType != ExpressionType.Parameter &&
                   expression.NodeType != ExpressionType.Lambda;
        }

        private object ExecuteCount(MongoQueryObject queryObject)
        {
            var miGetCollection = typeof(IMongoDatabase).GetMethods().Where(m => m.Name == "GetCollection" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1).Single().MakeGenericMethod(queryObject.DocumentType);
            var collection = miGetCollection.Invoke(queryObject.Database, new[] { queryObject.CollectionName });

            if (queryObject.Query.Count == 0)
                return Convert.ToInt32(collection.GetType().GetMethod("Count", Type.EmptyTypes).Invoke(collection, null));

            return Convert.ToInt32(collection.GetType().GetMethod("Count", new[] { typeof(object) }).Invoke(collection, new[] { queryObject.Query }));
        }

        private object ExecuteFind(MongoQueryObject queryObject)
        {
            var miGetCollection = typeof(IMongoDatabase).GetMethods().Where(m => m.Name == "GetCollection" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1).Single().MakeGenericMethod(queryObject.DocumentType);
            var collection = miGetCollection.Invoke(queryObject.Database, new[] { queryObject.CollectionName });

            var cursor = collection.GetType().GetMethod("FindAll")
                            .Invoke(collection, null);
            var cursorType = cursor.GetType();
            cursorType.GetMethod("Spec", new[] { typeof(Document) }).Invoke(cursor, new object[] { queryObject.Query });
            cursorType.GetMethod("Fields", new[] { typeof(Document) }).Invoke(cursor, new object[] { queryObject.Fields });
            cursorType.GetMethod("Limit").Invoke(cursor, new object[] { queryObject.NumberToLimit });
            cursorType.GetMethod("Skip").Invoke(cursor, new object[] { queryObject.NumberToSkip });

            var executor = GetExecutor(queryObject.DocumentType, queryObject.Projector, queryObject.Aggregator, true);
            return executor.Compile().DynamicInvoke(cursor);
        }

        private static LambdaExpression GetExecutor(Type documentType, LambdaExpression projector, LambdaExpression aggregator, bool boxReturn)
        {
            var cursor = Expression.Parameter(typeof(ICursor<>).MakeGenericType(documentType), "cursor");
            Expression body = Expression.New(typeof(ProjectionReader<,>).MakeGenericType(documentType, projector.Body.Type).GetConstructors()[0], cursor, projector);
            if (aggregator != null)
                body = Expression.Invoke(aggregator, body);
            if (boxReturn && body.Type != typeof(object))
                body = Expression.Convert(body, typeof(object));

            return Expression.Lambda(body, cursor);
        }

        // attempt to isolate a sub-expression that accesses a Query<T> object
        private class RootQueryableFinder : MongoExpressionVisitor
        {
            private Expression _root;

            public Expression Find(Expression expression)
            {
                Visit(expression);
                return _root;
            }

            protected override Expression Visit(Expression exp)
            {
                Expression result = base.Visit(exp);

                if (this._root == null && result != null && typeof(IQueryable).IsAssignableFrom(result.Type))
                {
                    this._root = result;
                }

                return result;
            }
        }
    }
}