using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Configuration
{
    public interface IMongoConfiguration
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the serialization factory.
        /// </summary>
        /// <value>The serialization factory.</value>
        ISerializationFactory SerializationFactory { get; }
    }
}