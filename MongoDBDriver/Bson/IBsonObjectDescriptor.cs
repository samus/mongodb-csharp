using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    public interface IBsonObjectDescriptor
    {
        IEnumerable<BsonObjectProperty> GetPropertys(object obj);

        bool IsArray(object obj);

        bool IsObject(object obj);
    }
}