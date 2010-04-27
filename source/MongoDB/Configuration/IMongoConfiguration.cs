using MongoDB.Serialization;

namespace MongoDB.Configuration
{
    /// <summary>
    /// 
    /// </summary>
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