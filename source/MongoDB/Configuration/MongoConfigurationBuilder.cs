using System;
using System.Configuration;

using MongoDB.Configuration.Builders;
using MongoDB.Configuration.Mapping;

namespace MongoDB.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoConfigurationBuilder
    {
        private string _connectionString;
        private MappingStoreBuilder _mappingStoreBuilder;

        /// <summary>
        /// Builds the configuration.
        /// </summary>
        /// <returns></returns>
        public MongoConfiguration BuildConfiguration()
        {
            if (_mappingStoreBuilder == null)
                return new MongoConfiguration {
                    ConnectionString = _connectionString
                };

            return new MongoConfiguration { 
                ConnectionString = _connectionString, 
                MappingStore = _mappingStoreBuilder.BuildMappingStore()
            };
        }

        /// <summary>
        /// Builds the mapping store.
        /// </summary>
        public IMappingStore BuildMappingStore()
        {
            if (_mappingStoreBuilder == null)
                return new AutoMappingStore();

            return _mappingStoreBuilder.BuildMappingStore();
        }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public void ConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <param name="config">The config.</param>
        public void ConnectionString(Action<MongoConnectionStringBuilder> config)
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
        public void ReadConnectionStringFromAppSettings(string key)
        {
            _connectionString = ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Configures the mapping.
        /// </summary>
        /// <param name="config">The config.</param>
        public void Mapping(Action<MappingStoreBuilder> config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (_mappingStoreBuilder == null)
                _mappingStoreBuilder = new MappingStoreBuilder();

            config(_mappingStoreBuilder);            
        }
    }
}