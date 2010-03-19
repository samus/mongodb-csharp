using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public interface IObjectDescriptor2
    {
        IEnumerable<object> GetPropertys();

        string GetPropertyName(object property);

        object GetPropertyValue(object property);
    }
}