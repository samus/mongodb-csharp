using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver.Bson;
using MongoDB.Driver.Configuration.Mapping;
using MongoDB.Driver.Serialization;
using MongoDB.Driver.Serialization.Builders;
using MongoDB.Driver.Serialization.Descriptors;

namespace MongoDB.Driver.Serialization
{
    public class SerializationFactory : ISerializationFactory
    {
        public static readonly SerializationFactory Default = new SerializationFactory();

        private IMappingStore _mappingStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperSerializationFactory"/> class.
        /// </summary>
        public SerializationFactory()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperSerializationFactory"/> class.
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
        /// <param name="connection">The connection.</param>
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
        /// <param name="connection">The connection.</param>
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