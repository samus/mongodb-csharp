using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    public interface IBsonObjectDescriptor
    {
        object BeginObject(object instance);

        IEnumerable<string> GetPropertyNames(object instance);

        object BeginProperty(object instance, string name);

        void EndProperty(object instance, string name, object value);

        void EndObject(object instance);

        bool IsArray(object instance);

        bool IsObject(object instance);
    }
}