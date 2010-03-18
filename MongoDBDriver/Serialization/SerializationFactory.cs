using System;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Connections;

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
            TypeNameProvider = new QualifiedNameTypeNameProvider();
            Registry = new TypeRegistry(this);
        }

        /// <summary>
        /// Gets or sets the type name provider.
        /// </summary>
        /// <value>The type name provider.</value>
        public ITypeNameProvider TypeNameProvider { get; set; }

        /// <summary>
        /// Gets or sets the registry.
        /// </summary>
        /// <value>The registry.</value>
        public TypeRegistry Registry { get; private set; }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        IBsonObjectBuilder ISerializationFactory.GetBsonBuilder(Type rootType, Connection connection){
            return new BsonReflectionBuilder(this,rootType);
        }

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        IBsonObjectDescriptor ISerializationFactory.GetBsonDescriptor(Type rootType, Connection connection){
            return new BsonReflectionDescriptor(this);
        }

        /// <summary>
        /// Gets the object descriptor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IObjectDescriptor GetObjectDescriptor(Type type){
            return Registry.GetOrCreate(type);
        }
    }
}