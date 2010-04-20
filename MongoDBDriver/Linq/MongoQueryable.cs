using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq
{
    public static class MongoQueryable
    {
        public static DocumentQuery Key<T>(this T document, string key) where T : Document
        {
            return new DocumentQuery(document, key);
        }
    }
}