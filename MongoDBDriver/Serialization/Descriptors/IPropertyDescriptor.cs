using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal interface IPropertyDescriptor
    {
        IEnumerable<string> GetPropertyNames();

        KeyValuePair<Type, object> GetPropertyTypeAndValue(string name);
    }
}