using System;
using MongoDB.Configuration.Mapping;
using MongoDB.Serialization;

namespace MongoDB.Configuration
{
    /// <summary>
    /// </summary>
    public class MongoConfiguration
    {
        /// <summary>
        ///   MongoDB-CSharp default configuration.
        /// </summary>
        public static readonly MongoConfiguration Default = new MongoConfiguration();

        private string _connectionString;

        private IMappingStore _mappingStore;
        private bool _readLocalTime;
        private ISerializationFactory _serializationFactory;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoConfiguration" /> class.
        /// </summary>
        public MongoConfiguration()
        {
            IsModifiable = true;
            _connectionString = string.Empty;
            _mappingStore = new AutoMappingStore();
            _serializationFactory = new SerializationFactory(this);
            _readLocalTime = true;
        }

        ///<summary>
        ///</summary>
        public bool IsModifiable { get; private set; }

        /// <summary>
        ///   Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                EnsureModifiable();
                _connectionString = value;
            }
        }

        /// <summary>
        ///   Gets or sets the serialization factory.
        /// </summary>
        /// <value>The serialization factory.</value>
        public ISerializationFactory SerializationFactory
        {
            get { return _serializationFactory; }
            set
            {
                EnsureModifiable();
                _serializationFactory = value;
            }
        }

        /// <summary>
        ///   Gets or sets the mapping store.
        /// </summary>
        /// <value>The mapping store.</value>
        public IMappingStore MappingStore
        {
            get { return _mappingStore; }
            set
            {
                EnsureModifiable();
                _mappingStore = value;
            }
        }

        /// <summary>
        ///   Reads DataTime from server as local time.
        /// </summary>
        /// <value><c>true</c> if [read local time]; otherwise, <c>false</c>.</value>
        /// <remarks>
        ///   MongoDB stores all time values in UTC timezone. If true the
        ///   time is converted from UTC to local timezone after is was read.
        /// </remarks>
        public bool ReadLocalTime
        {
            get { return _readLocalTime; }
            set
            {
                EnsureModifiable();
                _readLocalTime = value;
            }
        }

        /// <summary>
        ///   Ensures the modifiable.
        /// </summary>
        private void EnsureModifiable()
        {
            if(!IsModifiable)
                throw new InvalidOperationException("Value can not not be modified");
        }

        /// <summary>
        /// Validates the and seal.
        /// </summary>
        public void ValidateAndSeal()
        {
            if(ConnectionString == null)
                throw new MongoException("ConnectionString can not be null");
            if(MappingStore == null)
                throw new MongoException("MappingStore can not be null");
            if(SerializationFactory == null)
                throw new MongoException("SerializationFactory can not be null");

            IsModifiable = false;
        }
    }
}