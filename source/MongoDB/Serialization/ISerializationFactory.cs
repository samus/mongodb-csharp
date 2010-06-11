using System;
using MongoDB.Bson;

namespace MongoDB.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISerializationFactory
    {
        /// <summary>
        /// Gets the bson writer settings.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        BsonWriterSettings GetBsonWriterSettings(Type rootType);

        /// <summary>
        /// Gets the name of the collection given the rootType.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        string GetCollectionName(Type rootType);

        /// <summary>
        /// Gets the object descriptor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IObjectDescriptor GetObjectDescriptor(Type type);
     
        /// <summary>
        /// Gets the bson reader settings.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        BsonReaderSettings GetBsonReaderSettings(Type rootType);
    }
}