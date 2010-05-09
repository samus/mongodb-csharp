using System;
using MongoDB.Configuration.Mapping;
namespace MongoDB.Configuration
{
    public interface IMongoConfiguration
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the mapping store.
        /// </summary>
        /// <value>The mapping store.</value>
        IMappingStore MappingStore { get; }

        /// <summary>
        /// Gets a value indicating whether [read local time].
        /// </summary>
        /// <value><c>true</c> if [read local time]; otherwise, <c>false</c>.</value>
        bool ReadLocalTime { get;  }

        /// <summary>
        /// Gets the serialization factory.
        /// </summary>
        /// <value>The serialization factory.</value>
        MongoDB.Serialization.ISerializationFactory SerializationFactory { get; }
    }
}