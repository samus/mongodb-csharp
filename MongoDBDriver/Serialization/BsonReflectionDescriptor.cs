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
        private readonly Stack<Type> _type = new Stack<Type>();

        public BsonReflectionDescriptor(SerializationFactory serializationFactory,Type rootType){
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");
            if(rootType == null)
                throw new ArgumentNullException("rootType");
            
            _serializationFactory = serializationFactory;
            _type.Push(rootType);
        }

        public object BeginObject(object instance){
            if(instance is Document){
                return new DocumentDescriptor((Document)instance);
            }

            var expectedType = _type.Peek();
            var instanceType = instance.GetType();

            if(expectedType != instanceType)
                return new MappingObjectDescriptor(instance, instanceType, expectedType, _serializationFactory.Registry);
            
            return new ObjectDescriptor(instance, _serializationFactory.Registry);
        }

        public IEnumerable<string> GetPropertyNames(object instance){
            return ((IPropertyDescriptor)instance).GetPropertyNames();
        }

        public object BeginProperty(object instance, string name){
            var pair = ((IPropertyDescriptor)instance).GetPropertyValue(name);

            _type.Push(pair.Key);

            return pair.Value;
        }

        public void EndProperty(object instance, string name, object value){
            _type.Pop();
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