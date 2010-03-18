using System;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization
{
    public class SerializationFactory : ISerializationFactory
    {
        /// <summary>
        /// Default Factory for this application.
        /// </summary>
        public static readonly SerializationFactory Default = new SerializationFactory();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationFactory"/> class.
        /// </summary>
        public SerializationFactory(){
            Registry = new TypeRegistry();
        }

        /// <summary>
        /// Gets or sets the registry.
        /// </summary>
        /// <value>The registry.</value>
        public TypeRegistry Registry { get; private set; }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        IBsonObjectBuilder ISerializationFactory.GetBuilder(Type rootType){
            return new BsonReflectionBuilder(rootType);
        }

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <returns></returns>
        IBsonObjectDescriptor ISerializationFactory.GetDescriptor(){
            return new BsonReflectionDescriptor();
        }
    }
}