using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq
{
    public static class LinqExtensions
    {
        public static ICursor<T> Find<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> spec) where T : class
        {
            return collection.Find(GetQuery(collection, spec));
        }

        public static ICursor<T> Find<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> spec, int limit, int skip) where T : class
        {
            return collection.Find(GetQuery(collection, spec), limit, skip);
        }

        public static T FindOne<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> spec) where T : class
        {
            return collection.FindOne(GetQuery(collection, spec));
        }

        public static IQueryable<T> Linq<T>(this IMongoCollection<T> collection) where T : class
        {
            return new MongoQuery<T>(new MongoQueryProvider(collection.Database, collection.Name));
        }

        public static IQueryable<Document> Linq(this IMongoCollection collection)
        {
            return new MongoQuery<Document>(new MongoQueryProvider(collection.Database, collection.Name));
        }

        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector) where T : class
        {
            collection.Update(document, GetQuery(collection, selector));
        }

        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, bool safeMode) where T : class
        {
            collection.Update(document, GetQuery(collection, selector), safeMode);
        }

        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, UpdateFlags flags) where T : class
        {
            collection.Update(document, GetQuery(collection, selector), flags);
        }

        public static void Update<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, UpdateFlags flags, bool safeMode) where T : class
        {
            collection.Update(document, GetQuery(collection, selector), flags, safeMode);
        }

        public static void UpdateAll<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector) where T : class
        {
            collection.UpdateAll(document, GetQuery(collection, selector));
        }

        public static void UpdateAll<T>(this IMongoCollection<T> collection, object document, Expression<Func<T, bool>> selector, bool safeMode) where T : class
        {
            collection.UpdateAll(document, GetQuery(collection, selector), safeMode);
        }

        private static Document GetQuery<T>(IMongoCollection<T> collection, Expression<Func<T, bool>> spec) where T : class
        {
            var query = new MongoQuery<T>(new MongoQueryProvider(collection.Database, collection.Name))
                .Where(spec);
            var queryObject = ((IMongoQueryable)query).GetQueryObject();
            return queryObject.Query;
        }
    }
}