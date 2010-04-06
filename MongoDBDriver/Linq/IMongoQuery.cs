using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq
{
    internal interface IMongoQuery
    {
        MongoQueryObject GetQueryObject();
    }
}