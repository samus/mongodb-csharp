using System;

namespace MongoDB.Serialization.Builders
{
    internal interface IObjectBuilder
    {
        void AddProperty(string name, object value);

        object BuildObject();

        Type GetPropertyType(string name);
    }
}