using System;
using MongoDB.Bson;
using MongoDB.Configuration;

namespace MongoDB.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class SerializationFactory : ISerializationFactory
    {
        private readonly MongoConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationFactory"/> class.
        /// </summary>
        public SerializationFactory()
            : this(MongoConfiguration.Default)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationFactory"/> class.
        /// </summary>
        /// <param name="configuration">The mongo configuration.</param>
        public SerializationFactory(MongoConfiguration configuration)
        {
            if(configuration == null)
                throw new ArgumentNullException("configuration");

            _configuration = configuration;
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        public IBsonObjectBuilder GetBsonBuilder(Type rootType)
        {
            return new BsonClassMapBuilder(_configuration.MappingStore, rootType);
        }

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        public IBsonObjectDescriptor GetBsonDescriptor(Type rootType)
        {
            return new BsonClassMapDescriptor(_configuration.MappingStore, rootType);
        }

        /// <summary>
        /// Gets the bson reader settings.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        public BsonReaderSettings GetBsonReaderSettings(Type rootType)
        {
            return new BsonReaderSettings(GetBsonBuilder(rootType))
            {
                ReadLocalTime = _configuration.ReadLocalTime
            };
        }

        /// <summary>
        /// Gets the bson writer settings.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        public BsonWriterSettings GetBsonWriterSettings(Type rootType)
        {
            return new BsonWriterSettings(GetBsonDescriptor(rootType));
        }

        /// <summary>
        /// Gets the name of the collection given the rootType.
        /// </summary>
        /// <param name="rootType">Type of the root.</param>
        /// <returns></returns>
        public string GetCollectionName(Type rootType)
        {
            if (rootType == null)
                throw new ArgumentNullException("rootType");

            if (typeof(Document).IsAssignableFrom(rootType))
                throw new InvalidOperationException("Documents cannot have a default collection name.");

            return _configuration.MappingStore.GetClassMap(rootType).CollectionName;
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

            return new ClassMapObjectDescriptorAdapter(_configuration.MappingStore.GetClassMap(type));
        }
    }
}