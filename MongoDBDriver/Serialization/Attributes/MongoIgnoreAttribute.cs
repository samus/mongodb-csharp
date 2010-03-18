using System;

namespace MongoDB.Driver.Serialization.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MongoIgnoreAttribute : Attribute
    {
    }
}