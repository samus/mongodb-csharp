using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq
{
    public static class LinqExtensions
    {
        public static IQueryable<T> Linq<T>(this IMongoCollection<T> collection) where T : class
        {
            return new MongoQuery<T>(new MongoQueryProvider(collection.Database, collection.Name));
        }

        public static IQueryable<Document> Linq(this IMongoCollection collection)
        {
            return new MongoQuery<Document>(new MongoQueryProvider(collection.Database, collection.Name));
        }
    }
}