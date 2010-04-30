using MongoDB.Configuration.Mapping;
using MongoDB.Serialization;

namespace MongoDB.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoConfiguration
    {
        /// <summary>
        /// MongoDB-CSharp default configuration.
        /// </summary>
        public static readonly MongoConfiguration Default = new MongoConfiguration();

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConfiguration"/> class.
        /// </summary>
        public MongoConfiguration(){
            ConnectionString = string.Empty;
            MappingStore = new AutoMappingStore();
            SerializationFactory = new SerializationFactory(MappingStore);
        }

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

        /// <summary>
        /// Gets or sets the mapping store.
        /// </summary>
        /// <value>The mapping store.</value>
        public IMappingStore MappingStore { get; set; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate(){
            if(ConnectionString == null)
                throw new MongoException("ConnectionString can not be null");
            if(MappingStore == null)
                throw new MongoException("MappingStore can not be null");
            if(SerializationFactory == null)
                throw new MongoException("SerializationFactory can not be null");
        }
    }
}
