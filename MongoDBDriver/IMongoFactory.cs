using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver
{
    public interface IMongoFactory
    {
        Mongo CreateMongo();
    }
}