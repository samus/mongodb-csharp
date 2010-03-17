using System;

namespace MongoDB.Driver.Serialization.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MongoNameAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MongoNameAttribute(string name){
            if(name == null)
                throw new ArgumentNullException("name");
            Name = name;
        }
    }
}