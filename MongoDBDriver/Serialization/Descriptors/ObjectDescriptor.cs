using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public class ObjectDescriptor : IPropertyDescriptor
    {
        private readonly object _instance;
        private readonly TypeEntry _entry;

        public ObjectDescriptor(object instance, TypeRegistry registry){
            _instance = instance;
            var type = _instance.GetType();
            _entry = registry.GetOrCreate(type);
        }

        public IEnumerable<string> GetPropertyNames(){
            return _entry.GetMongoPropertyNames();
        }

        public KeyValuePair<Type, object> GetPropertyValue(string name){
            var property = _entry.GetPropertyFromMongoName(name);
            var value = property.GetValue(_instance);
            return new KeyValuePair<Type, object>(property.PropertyType,value);
        }
    }
}