using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver
{
    public class MongoFactory : IMongoFactory
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the serialization factory.
        /// </summary>
        /// <value>The serialization factory.</value>
        public ISerializationFactory SerializationFactory { get; set; }

        /// <summary>
        /// Creates the mongo.
        /// </summary>
        /// <returns></returns>
        public Mongo CreateMongo()
        {
            if (ConnectionString == null && SerializationFactory == null)
                return new Mongo();
            else if (SerializationFactory == null)
                return new Mongo(ConnectionString);
            else if (ConnectionString == null)
                return new Mongo(SerializationFactory);

            return new Mongo(ConnectionString, SerializationFactory);
        }
    }
}
