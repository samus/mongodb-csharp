using System;
using MongoDB.Configuration.Mapping;
using MongoDB.Serialization;

namespace MongoDB.Configuration
{
    /// <summary>
    /// </summary>
    public class MongoConfiguration
    {
        private static MongoConfiguration _default;

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

        /// <summary>
        ///   Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static MongoConfiguration Default
        {
            get
            {
                if(_default == null)
                {
                    var configuration = new MongoConfiguration();
                    var section = MongoConfigurationSection.GetSection();
                    if(section != null)
                        section.UpdateConfiguration(configuration);
                    _default = configuration;
                }

                return _default;
            }
        }

        /// <summary>
        /// Clones the specified configuration with a new 
        /// connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public MongoConfiguration Clone(string connectionString)
        {
            return new MongoConfiguration
            {
                ConnectionString = connectionString,
                MappingStore = _mappingStore,
                ReadLocalTime = _readLocalTime,
                SerializationFactory = _serializationFactory,
                IsModifiable = false
            };
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
                TryModify();
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
                TryModify();
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
                TryModify();
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
                TryModify();
                _readLocalTime = value;
            }
        }

        /// <summary>
        ///   Ensures the modifiable.
        /// </summary>
        protected void TryModify()
        {
            if(!IsModifiable)
                throw new InvalidOperationException("Value can not not be modified");
        }

        /// <summary>
        ///   Validates the and seal.
        /// </summary>
        public virtual void ValidateAndSeal()
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