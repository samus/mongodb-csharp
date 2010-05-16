using System;

namespace MongoDB.Serialization.Builders
{
    internal interface IObjectBuilder
    {
        void AddProperty(string name, object value);

        object BuildObject();

        PropertyDescriptor GetPropertyDescriptor(string name);
    }
}