using System;
using System.Configuration;

using MongoDB.Driver.Configuration.Mapping;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class FluentConfiguration : IMongoConfiguration, IMappingConfiguration
    {
        private string _connectionString;
        private MappingConfiguration _mappingConfiguration;

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// Gets the mapping store.
        /// </summary>
        /// <value>The mapping store.</value>
        public IMappingStore MappingStore
        {
            get
            {
                if (_mappingConfiguration == null)
                    return new AutoMappingStore();

                return _mappingConfiguration.MappingStore;
            }
        }

        /// <summary>
        /// Gets the serialization factory.
        /// </summary>
        /// <value>The serialization factory.</value>
        public ISerializationFactory SerializationFactory
        {
            get
            {
                if (_mappingConfiguration == null)
                    return Serialization.SerializationFactory.Default;

                return new SerializationFactory(MappingStore);
            }
        }

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <param name="config">The config.</param>
        public void BuildConnectionString(Action<MongoConnectionStringBuilder> config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            var builder = new MongoConnectionStringBuilder();
            config(builder);
            _connectionString = builder.ToString();
        }

        /// <summary>
        /// Set the apps settings key from which to pull the connection string,
        /// </summary>
        /// <param name="key">The key.</param>
        public void ConnectionStringAppSettingKey(string key)
        {
            _connectionString = ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Configures the mapping.
        /// </summary>
        /// <param name="config">The config.</param>
        public void Mapping(Action<MappingConfiguration> config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (_mappingConfiguration == null)
                _mappingConfiguration = new MappingConfiguration();

            config(_mappingConfiguration);            
        }
    }
}