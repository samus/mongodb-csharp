using System;
using System.Configuration;

namespace MongoDB.Driver.Configuration
{
    public class MongoConfiguration : ConfigurationSection
    {

        public MongoConfiguration() { }

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
