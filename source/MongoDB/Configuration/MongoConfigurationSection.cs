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
        ///   Reads DataTime from server as local time.
        /// </summary>
        /// <value><c>true</c> if [read local time]; otherwise, <c>false</c>.</value>
        /// <remarks>
        ///   MongoDB stores all time values in UTC timezone. If true the
        ///   time is converted from UTC to local timezone after is was read.
        /// </remarks>
        [ConfigurationProperty("readLocalTime", DefaultValue = true)]
        public bool ReadLocalTime
        {
            get{return (bool)this["readLocalTime"];}
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
                    configuration.ConnectionString = connection.ConnectionString;
            }

            configuration.ReadLocalTime = ReadLocalTime;
        }
    }
}
