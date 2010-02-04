using System;
using System.Configuration;
using System.Text;

namespace MongoDB.Driver.Configuration
{
    public class MongoDBConfiguration : ConfigurationSection
    {

        public MongoDBConfiguration() { }

        [ConfigurationProperty("port", DefaultValue = "27017", IsRequired = true)]
        [IntegerValidator(MinValue = 1, MaxValue = 65535, ExcludeRange = false)]
        public int Port
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("host", DefaultValue = "localhost", IsRequired = true)]
         public string Host {
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

    /// <summary>
    /// Not used at the moment but could be used later
    /// </summary>
    public class ChildConfigElement : ConfigurationElement
    {
        public ChildConfigElement() { }
    }

}
