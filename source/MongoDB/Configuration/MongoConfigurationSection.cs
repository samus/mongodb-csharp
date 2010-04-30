using System.Configuration;

namespace MongoDB.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoConfigurationSection : ConfigurationSection
    {
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
            return GetSection("Mongo");
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
    }
}
