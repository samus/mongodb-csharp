using System;

namespace MongoDB.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MongoIdAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoIdAttribute"/> class.
        /// </summary>
        public MongoIdAttribute()
        { }
    }
}