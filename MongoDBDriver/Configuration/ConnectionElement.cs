using System;
using System.Configuration;

namespace MongoDB.Driver.Configuration
{
    
    public class ConnectionElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Name{
            get{return (String)this["key"];}
            set{this["key"] = value;}
        }
        
        [ConfigurationProperty("connectionString", DefaultValue = "Server=localhost:27017")]
        public string ConnectionString{
            get { return (String)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }
    }
}
