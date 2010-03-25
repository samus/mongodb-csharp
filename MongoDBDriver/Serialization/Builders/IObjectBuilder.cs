using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Builders
{
    internal interface IObjectBuilder
    {
        void AddProperty(string name, object value);

        object BuildObject();

        Type GetPropertyType(string name);
    }
}