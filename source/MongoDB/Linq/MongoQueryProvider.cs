using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Connections;
using MongoDB.Linq.Expressions;
using MongoDB.Serialization;

namespace MongoDB.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoQueryProvider : IQueryProvider
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public string CollectionName
        {
            get { return _collectionName; }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public IMongoDatabase Database
        {
            get { return _database; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoQueryProvider"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        public MongoQueryProvider(IMongoDatabase database, string collectionName)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (collectionName == null)
                throw new ArgumentNullException("collectionName");

            _collectionName = collectionName;
            _database = database;
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MongoQuery<TElement>(this, expression);
        }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable"/> object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable"/> that can evaluate the query represented by the specified expression tree.
        /// </returns>
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

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public TResult Execute<TResult>(Expression expression)
        {
            object result = Execute(expression);
            return (TResult)result;
        }

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// The value that results from executing the specified query.
        /// </returns>
        public object Execute(Expression expression)
        {
            var queryObject = GetQueryObject(expression);
            return ExecuteQueryObject(queryObject);
        }

        /// <summary>
        /// Executes the query object.
        /// </summary>
        /// <param name="queryObject">The query object.</param>
        /// <returns></returns>
        internal object ExecuteQueryObject(MongoQueryObject queryObject){
            if (queryObject.IsCount)
                return ExecuteCount(queryObject);
            else if (queryObject.IsMapReduce)
                return ExecuteMapReduce(queryObject);
            return ExecuteFind(queryObject);
        }

        /// <summary>
        /// Gets the query object.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal MongoQueryObject GetQueryObject(Expression expression)
        {
            var projection = expression as ProjectionExpression;
            if(projection == null)
            {
                expression = PartialEvaluator.Evaluate(expression, CanBeEvaluatedLocally);
                expression = new FieldBinder().Bind(expression);
                expression = new QueryBinder(this, expression).Bind(expression);
                expression = new AggregateRewriter().Rewrite(expression);
                expression = new RedundantSubqueryRemover().Remove(expression);
                expression = new OrderByRewriter().Rewrite(expression);
                expression = new RedundantSubqueryRemover().Remove(expression);
                projection = (ProjectionExpression)expression;
            }

            return new MongoQueryObjectBuilder().Build(projection);
        }

        /// <summary>
        /// Determines whether this instance [can be evaluated locally] the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can be evaluated locally] the specified expression; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Executes the count.
        /// </summary>
        /// <param name="queryObject">The query object.</param>
        /// <returns></returns>
        private object ExecuteCount(MongoQueryObject queryObject)
        {
            var miGetCollection = typeof(IMongoDatabase).GetMethods().Where(m => m.Name == "GetCollection" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1).Single().MakeGenericMethod(queryObject.DocumentType);
            var collection = miGetCollection.Invoke(queryObject.Database, new[] { queryObject.CollectionName });

            if (queryObject.Query == null)
                return Convert.ToInt32(collection.GetType().GetMethod("Count", Type.EmptyTypes).Invoke(collection, null));

            return Convert.ToInt32(collection.GetType().GetMethod("Count", new[] { typeof(object) }).Invoke(collection, new[] { queryObject.Query }));
        }

        /// <summary>
        /// Executes the select.
        /// </summary>
        /// <param name="queryObject">The query object.</param>
        /// <returns></returns>
        private object ExecuteFind(MongoQueryObject queryObject)
        {
            var miGetCollection = typeof(IMongoDatabase).GetMethods().Where(m => m.Name == "GetCollection" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1).Single().MakeGenericMethod(queryObject.DocumentType);
            var collection = miGetCollection.Invoke(queryObject.Database, new[] { queryObject.CollectionName });

            var cursor = collection.GetType().GetMethod("FindAll")
                            .Invoke(collection, null);
            var cursorType = cursor.GetType();
            Document spec;
            if (queryObject.Sort != null)
            {
                spec = new Document();
                spec.Add("query", queryObject.Query);
                spec.Add("orderby", queryObject.Sort);
            }
            else
                spec = queryObject.Query;

            cursorType.GetMethod("Spec", new[] { typeof(Document) }).Invoke(cursor, new object[] { spec });
            cursorType.GetMethod("Fields", new[] { typeof(Document) }).Invoke(cursor, new object[] { queryObject.Fields });
            cursorType.GetMethod("Limit").Invoke(cursor, new object[] { queryObject.NumberToLimit });
            cursorType.GetMethod("Skip").Invoke(cursor, new object[] { queryObject.NumberToSkip });

            var executor = GetExecutor(queryObject.DocumentType, queryObject.Projector, queryObject.Aggregator, true);
            return executor.Compile().DynamicInvoke(cursor.GetType().GetProperty("Documents").GetValue(cursor, null));
        }

        /// <summary>
        /// Executes the map reduce.
        /// </summary>
        /// <param name="queryObject">The query object.</param>
        /// <returns></returns>
        private object ExecuteMapReduce(MongoQueryObject queryObject)
        {
            var miGetCollection = typeof(IMongoDatabase).GetMethods().Where(m => m.Name == "GetCollection" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1).Single().MakeGenericMethod(queryObject.DocumentType);
            var collection = miGetCollection.Invoke(queryObject.Database, new[] { queryObject.CollectionName });

            var mapReduce = (MapReduce)collection.GetType().GetMethod("MapReduce").Invoke(collection, null);
            mapReduce.Map = new Code(queryObject.MapFunction);
            mapReduce.Reduce = new Code(queryObject.ReduceFunction);
            mapReduce.Finalize = new Code(queryObject.FinalizerFunction);
            mapReduce.Query = queryObject.Query;

            if(queryObject.Sort != null)
                mapReduce.Sort = queryObject.Sort;

            mapReduce.Limit = queryObject.NumberToLimit;
            if (queryObject.NumberToSkip != 0)
                throw new InvalidQueryException("MapReduce queries do no support Skips.");

            var executor = GetExecutor(typeof(Document), queryObject.Projector, queryObject.Aggregator, true);
            return executor.Compile().DynamicInvoke(mapReduce.Documents);
        }

        /// <summary>
        /// Gets the executor.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="projector">The projector.</param>
        /// <param name="aggregator">The aggregator.</param>
        /// <param name="boxReturn">if set to <c>true</c> [box return].</param>
        /// <returns></returns>
        private static LambdaExpression GetExecutor(Type documentType, LambdaExpression projector, LambdaExpression aggregator, bool boxReturn)
        {
            var documents = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(documentType), "documents");
            Expression body = Expression.New(typeof(ProjectionReader<,>).MakeGenericType(documentType, projector.Body.Type).GetConstructors()[0], documents, projector);
            if (aggregator != null)
                body = Expression.Invoke(aggregator, body);
            if (boxReturn && body.Type != typeof(object))
                body = Expression.Convert(body, typeof(object));

            return Expression.Lambda(body, documents);
        }

        /// <summary>
        /// attempt to isolate a sub-expression that accesses a Query object
        /// </summary>
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