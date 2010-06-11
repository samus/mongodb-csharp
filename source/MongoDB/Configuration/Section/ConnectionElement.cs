using System;
using System.Configuration;

namespace MongoDB.Configuration.Section
{

    /// <summary>
    /// 
    /// </summary>
    public class ConnectionElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("key", IsRequired = true)]
        public string Name{
            get{return (String)this["key"];}
            set{this["key"] = value;}
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        [ConfigurationProperty("connectionString", DefaultValue = "Server=localhost:27017")]
        public string ConnectionString{
            get { return (String)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
        {
            get { return string.IsNullOrEmpty(Name) || Name.EndsWith("default", StringComparison.InvariantCultureIgnoreCase); }
        }
    }
}
