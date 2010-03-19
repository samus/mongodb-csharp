using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public interface IPropertyDescriptor
    {
        IEnumerable<string> GetPropertyNames();

        KeyValuePair<Type,object> GetPropertyValue(string name);
    }
}