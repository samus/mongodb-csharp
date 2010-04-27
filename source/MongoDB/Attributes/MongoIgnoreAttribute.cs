using System;

namespace MongoDB.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MongoIgnoreAttribute : Attribute
    {
    }
}