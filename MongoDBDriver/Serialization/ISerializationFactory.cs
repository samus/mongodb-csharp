using System;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization
{
    public interface ISerializationFactory
    {
        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        IBsonObjectBuilder GetBuilder(Type rootType);

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <returns></returns>
        IBsonObjectDescriptor GetDescriptor();
    }
}