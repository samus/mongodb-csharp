using System;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Connections;

namespace MongoDB.Driver.Serialization
{
    public interface ISerializationFactory
    {
        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        IBsonObjectBuilder GetBuilder(Type rootType,Connection connection);

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        IBsonObjectDescriptor GetDescriptor(Type rootType, Connection connection);
    }
}