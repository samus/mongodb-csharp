using MongoDB.Driver.Serialization;

namespace MongoDB.Driver
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMongoConfiguration
    {
        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <returns></returns>
        string BuildConnectionString();

        /// <summary>
        /// Builds the serialization factory.
        /// </summary>
        /// <returns></returns>
        ISerializationFactory BuildSerializationFactory();
    }
}