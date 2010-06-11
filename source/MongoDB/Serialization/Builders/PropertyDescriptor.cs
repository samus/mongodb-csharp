using System;

namespace MongoDB.Serialization.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyDescriptor
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dictionary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dictionary; otherwise, <c>false</c>.
        /// </value>
        public bool IsDictionary { get; set; }
    }
}
