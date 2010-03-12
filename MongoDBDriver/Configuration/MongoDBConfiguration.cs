using System;
using System.Configuration;
using System.Text;

namespace MongoDB.Driver.Configuration
{
    public class MongoDBConfiguration : ConfigurationSection
    {

        public MongoDBConfiguration() { }

        [ConfigurationProperty("connection")]
        public ConnectionElement Connection{
            get{return (ConnectionElement)this["connection"];}
            set{ this["connection"] = value; }
        }

        [ConfigurationProperty("gridFS")]
        public GridFSElement GridFS
        {
            get { return (GridFSElement)this["gridFS"]; }
            set { this["gridFS"] = value; }
        }

    }


    public class ConnectionElement : ConfigurationElement
    {

        [ConfigurationProperty("port", DefaultValue = "27017", IsRequired = true)]
        [IntegerValidator(MinValue = 1, MaxValue = 65535, ExcludeRange = false)]
        public int Port{
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("host", DefaultValue = "localhost", IsRequired = true)]
        public string Host{
            get { return (String)this["host"]; }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("database", IsRequired = true)]
        public string Database{
            get { return (String)this["database"]; }
            set { this["database"] = value; }
        }

        [ConfigurationProperty("use_authentication", DefaultValue = false, IsRequired = false)]
        public bool UseAuthentication{
            get { return (Boolean)this["use_authentication"]; }
            set { this["use_authentication"] = value; }
        }

        [ConfigurationProperty("username", DefaultValue = "", IsRequired = false)]
        public string Username{
            get { return (String)this["username"]; }
            set { this["username"] = value; }
        }

        [ConfigurationProperty("password", DefaultValue = "", IsRequired = false)]
        public string Password{
            get { return (String)this["password"]; }
            set { this["password"] = value; }
        }
    }

    public class GridFSElement : ConfigurationElement
    {
        [ConfigurationProperty("collection", IsRequired = false, DefaultValue="fs")]
        public string Collection
        {
            get { return (String)this["collection"]; }
            set { this["collection"] = value; }
        }

        [ConfigurationProperty("chunkSize", IsRequired = false, DefaultValue="256000")]
        public Int32 ChunkSize
        {
            get { return (Int32)this["chunkSize"]; }
            set { this["chunkSize"] = value; }
        }

    }

}
