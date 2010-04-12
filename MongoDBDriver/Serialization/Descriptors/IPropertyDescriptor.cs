using System;
using System.Collections.Generic;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal interface IPropertyDescriptor
    {
        IEnumerable<BsonProperty> GetProperties();
    }
}