using System;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MongoIgnoreAttribute : Attribute
    {
    }
}