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

        public IEnumerable<string> GetPropertyNames(object instance){
            return ((IObjectDescriptor2)instance).GetPropertyNames();
        }

        public object BeginProperty(object instance, string name){
            return ((IObjectDescriptor2)instance).GetPropertyValue(name);
        }

        public void EndProperty(object instance, string name, object value){
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