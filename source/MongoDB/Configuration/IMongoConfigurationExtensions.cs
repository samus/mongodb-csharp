using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Configuration
{
    public static class IMongoConfigurationExtensions
    {
        public static void Validate(this IMongoConfiguration configuration)
        {
            if (configuration.ConnectionString == null)
                throw new MongoConfigurationException("ConnectionString can not be null");
            if (configuration.MappingStore == null)
                throw new MongoConfigurationException("MappingStore can not be null");
            if (configuration.SerializationFactory == null)
                throw new MongoConfigurationException("SerializationFactory can not be null");
        }
    }
}