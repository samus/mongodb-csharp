using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization
{
    public class ReflectionDescriptor : IBsonObjectDescriptor
    {
        public IEnumerable<BsonObjectProperty> GetPropertys(object obj)
        {
            var type = obj.GetType();
            foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(type))
                yield return new BsonObjectProperty(property.Name, property.GetValue(obj));
        }

        public bool IsArray(object obj)
        {
            return obj is IEnumerable;
        }

        public bool IsObject(object obj)
        {
            return true;
        }
    }
}