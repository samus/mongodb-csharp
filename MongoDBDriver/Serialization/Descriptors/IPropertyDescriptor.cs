using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal interface IPropertyDescriptor
    {
        IEnumerable<KeyValuePair<string, KeyValuePair<Type, object>>> GetProperties();
    }
}