using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal interface IPropertyDescriptor
    {
        IEnumerable<string> GetPropertyNames();

        KeyValuePair<Type, object> GetPropertyTypeAndValue(string name);
    }
}