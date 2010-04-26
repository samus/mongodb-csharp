using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping;
using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Serialization;
using System.Configuration;

namespace MongoDB.Driver.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoConfiguration : IMongoConfiguration, IMappingConfiguration
    {
        private string _connectionString;
        private MappingConfiguration _mappingConfiguration;

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public void ConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Configures the connection string.
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

        /// <summary>
        /// Builds the mapping store.
        /// </summary>
        /// <returns></returns>
        IMappingStore IMappingConfiguration.BuildMappingStore()
        {
            if (_mappingConfiguration == null)
                return new AutoMappingStore();

            return ((IMappingConfiguration)_mappingConfiguration).BuildMappingStore();
        }

        /// <summary>
        /// Builds the serialization factory.
        /// </summary>
        /// <returns></returns>
        ISerializationFactory IMappingConfiguration.BuildSerializationFactory()
        {
            if (_mappingConfiguration == null)
                return SerializationFactory.Default;

            return new SerializationFactory(((IMappingConfiguration)this).BuildMappingStore());
        }

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <returns></returns>
        string IMongoConfiguration.BuildConnectionString()
        {
            return _connectionString;
        }

        /// <summary>
        /// Builds the serialization factory.
        /// </summary>
        /// <returns></returns>
        ISerializationFactory IMongoConfiguration.BuildSerializationFactory()
        {
            return ((IMappingConfiguration)this).BuildSerializationFactory();
        }
    }
}