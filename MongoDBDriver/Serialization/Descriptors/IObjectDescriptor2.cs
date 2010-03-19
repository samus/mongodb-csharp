using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public interface IObjectDescriptor2
    {
        IEnumerable<string> GetPropertyNames();

        object GetPropertyValue(string name);
    }
}