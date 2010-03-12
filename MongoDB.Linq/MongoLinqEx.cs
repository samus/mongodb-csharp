using System;
using MongoDB.Driver;

namespace MongoDB.Linq
{
	public static class MongoLinqEx
	{
        public static IMongoQuery AsQueryable<T>(this T collection) where T : IMongoCollection<Document>
		{
			return new MongoQuery(new MongoQueryProvider(collection));
		}

        public static MongoDocumentQuery Key<T>(this T document,string key) where T: Document{
            return new MongoDocumentQuery(document,key);
        }
	}
}
