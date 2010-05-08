using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
{
    internal interface IPropertyDescriptor
    {
        IEnumerable<BsonProperty> GetProperties();
    }
}