using System;

namespace MongoDB.Driver.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MongoAliasAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoAliasAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MongoAliasAttribute(string name){
            if(name == null)
                throw new ArgumentNullException("name");
            Name = name;
        }
    }
}