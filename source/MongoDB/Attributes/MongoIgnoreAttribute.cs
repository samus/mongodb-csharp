using System;

namespace MongoDB.Driver.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MongoIgnoreAttribute : Attribute
    {
    }
}