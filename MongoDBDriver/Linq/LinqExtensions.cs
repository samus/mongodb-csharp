using System;
using System.Linq;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Finds the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public static ICursor<T> Find<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> spec) where T : class
        {
            return collection.Find(GetQuery(collection, spec));
        }

        /// <summary>
        /// Finds the one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public static T FindOne<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> spec) where T : class
        {
            return collection.FindOne(GetQuery(collection, spec));
        }

        /// <summary>
        /// Linqs the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static IQueryable<T> Linq<T>(this IMongoCollection<T> collection) where T : class
        {
            return new MongoQuery<T>(new MongoQueryProvider(collection.Database, collection.Name));
        }

        /// <summary>
        /// Linqs the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static IQueryable<Document> Linq(this IMongoCollection collection)
        {
            return new MongoQuery<Document>(new MongoQueryProvider(collection.Database, collection.Name));
        }

        /// <summary>
        /// Updates the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector) where T : class
        {
            collection.Update(document, GetQuery(collection, selector));
        }

        /// <summary>
        /// Updates the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safeMode">if set to <c>true</c> [safe mode].</param>
        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, bool safeMode) where T : class
        {
            collection.Update(document, GetQuery(collection, selector), safeMode);
        }

        /// <summary>
        /// Updates the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="flags">The flags.</param>
        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, UpdateFlags flags) where T : class
        {
            collection.Update(document, GetQuery(collection, selector), flags);
        }

        /// <summary>
        /// Updates the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="safeMode">if set to <c>true</c> [safe mode].</param>
        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, UpdateFlags flags, bool safeMode) where T : class
        {
            collection.Update(document, GetQuery(collection, selector), flags, safeMode);
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public static void UpdateAll<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector) where T : class
        {
            collection.UpdateAll(document, GetQuery(collection, selector));
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safeMode">if set to <c>true</c> [safe mode].</param>
        public static void UpdateAll<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, bool safeMode) where T : class
        {
            collection.UpdateAll(document, GetQuery(collection, selector), safeMode);
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        private static Document GetQuery<T>(IMongoCollection<T> collection, Expression<Func<T, bool>> spec) where T : class
        {
            var query = new MongoQuery<T>(new MongoQueryProvider(collection.Database, collection.Name))
                .Where(spec);
            var queryObject = ((IMongoQueryable)query).GetQueryObject();
            return queryObject.Query;
        }
    }
}