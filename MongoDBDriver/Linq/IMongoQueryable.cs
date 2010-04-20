using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq
{
    internal interface IMongoQueryable : IQueryable
    {
        string CollectionName { get; }

        IMongoDatabase Database { get; }

        MongoQueryObject GetQueryObject();
    }
}