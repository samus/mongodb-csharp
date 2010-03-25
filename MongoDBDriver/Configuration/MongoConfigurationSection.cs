using System.Configuration;

namespace MongoDB.Driver.Configuration
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
    }
}
