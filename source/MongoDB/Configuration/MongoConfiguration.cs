using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoConfiguration : IMongoConfiguration
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
    }
}
