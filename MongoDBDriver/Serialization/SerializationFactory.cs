using System;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class SerializationFactory : ISerializationFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly SerializationFactory Default = new SerializationFactory();

        private readonly IMappingStore _mappingStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationFactory"/> class.
        /// </summary>
        public SerializationFactory()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationFactory"/> class.
        /// </summary>
        /// <param name="mappingStore">The mapping store.</param>
        public SerializationFactory(IMappingStore mappingStore)
        {
            _mappingStore = mappingStore ?? new AutoMappingStore();
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        public IBsonObjectBuilder GetBsonBuilder(Type rootType)
        {
            if (typeof(Document).IsAssignableFrom(rootType))
                return new BsonDocumentBuilder();

            return new BsonClassMapBuilder(_mappingStore, rootType);
        }

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        public IBsonObjectDescriptor GetBsonDescriptor(Type rootType)
        {
            if (typeof(Document).IsAssignableFrom(rootType))
                return new BsonDocumentDescriptor();

            return new BsonClassMapDescriptor(_mappingStore, rootType);
        }

        /// <summary>
        /// Gets the object descriptor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IObjectDescriptor GetObjectDescriptor(Type type)
        {
            if (typeof(Document).IsAssignableFrom(type))
                return new DocumentObjectDescriptorAdapter();

            return new ClassMapObjectDescriptorAdapter(_mappingStore.GetClassMap(type));
        }
    }
}