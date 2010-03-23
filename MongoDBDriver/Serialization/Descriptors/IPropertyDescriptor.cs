using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyDescriptor
    {
        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetPropertyNames();

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        KeyValuePair<Type,object> GetPropertyValue(string name);
    }
}