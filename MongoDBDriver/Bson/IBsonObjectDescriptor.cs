using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    public interface IBsonObjectDescriptor
    {
        object BeginObject(object instance);

        IEnumerable<object> GetPropertys(object instance);

        string GetPropertyName(object instance, object property);

        object GetPropertyValue(object instance, object property);

        void EndObject(object instance);

        bool IsArray(object instance);

        bool IsObject(object instance);
    }
}