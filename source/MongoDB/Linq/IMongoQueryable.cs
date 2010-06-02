using System.Linq;

namespace MongoDB.Linq
{
    internal interface IMongoQueryable : IQueryable
    {
        string CollectionName { get; }

        IMongoDatabase Database { get; }

        MongoQueryObject GetQueryObject();
    }
}