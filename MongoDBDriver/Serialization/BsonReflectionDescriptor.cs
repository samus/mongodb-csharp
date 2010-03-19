using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization.Descriptors;

namespace MongoDB.Driver.Serialization
{
    public class BsonReflectionDescriptor : IBsonObjectDescriptor
    {
        private readonly SerializationFactory _serializationFactory;

        public BsonReflectionDescriptor(SerializationFactory serializationFactory){
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");
            _serializationFactory = serializationFactory;
        }

        public object BeginObject(object instance){
            if(instance is Document){
                return new DocumentDescriptor((Document)instance);
            }
            
            var type = instance.GetType();
            var entry = _serializationFactory.Registry.GetOrCreate(type);
            
            return new ObjectDescriptor(instance,entry);
        }

        public IEnumerable<object> GetPropertys(object instance){
            var descriptor = (IObjectDescriptor2)instance;
            return descriptor.GetPropertys();
        }

        public string GetPropertyName(object instance, object property)
        {
            var descriptor = (IObjectDescriptor2)instance;
            return descriptor.GetPropertyName(property);
        }

        public object GetPropertyValue(object instance, object property){
            var descriptor = (IObjectDescriptor2)instance;
            return descriptor.GetPropertyValue(property);
        }

        public void EndObject(object obj){
        }

        public bool IsArray(object obj)
        {
            if(obj is Document)
                return false;

            return obj is IEnumerable;
        }

        public bool IsObject(object obj)
        {
            return true;
        }
    }
}