using System;
using System.Configuration;
using System.Linq;

namespace MongoDB.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DefaultSectionName = "mongo";

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        [ConfigurationProperty("connections", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ConnectionCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ConnectionCollection Connections{
            get{return (ConnectionCollection)this["connections"];}
        }

        /// <summary>
        /// Gets the section with name Mongo.
        /// </summary>
        /// <returns></returns>
        public static MongoConfigurationSection GetSection()
        {
            return GetSection(DefaultSectionName);
        }

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static MongoConfigurationSection GetSection(string name)
        {
            return ConfigurationManager.GetSection(name) as MongoConfigurationSection;
        }

        /// <summary>
        /// Creates the configuration.
        /// </summary>
        /// <returns></returns>
        public MongoConfiguration CreateConfiguration()
        {
            var configuration = new MongoConfiguration();

            UpdateConfiguration(configuration);

            return configuration;
        }

        /// <summary>
        /// Updates the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void UpdateConfiguration(MongoConfiguration configuration)
        {
            if(configuration == null)
                throw new ArgumentNullException("configuration");

            if(Connections!=null)
            {
                var connection = Connections.Cast<ConnectionElement>().FirstOrDefault(c=>c.IsDefault);
                if(connection != null)
                    connection.ConnectionString = connection.ConnectionString;
            }
        }
    }
}
