using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Configuration.Mapping;
using MongoDB.Serialization.Descriptors;

namespace MongoDB.Serialization
{
    internal class BsonClassMapDescriptor : IBsonObjectDescriptor
    {
        private readonly Stack<Type> _types;
        private readonly IMappingStore _mappingStore;
        private bool _isMapReduce;

        public BsonClassMapDescriptor(IMappingStore mappingStore, Type rootType)
        {
            if (mappingStore == null)
                throw new ArgumentNullException("mappingStore");
            if (rootType == null)
                throw new ArgumentNullException("rootType");

            _mappingStore = mappingStore;
            _types = new Stack<Type>();
            _types.Push(rootType);
        }

        public object BeginObject(object instance)
        {
            if (instance is Document)
                return BeginDocument((Document)instance);

            var currentClassMap = _mappingStore.GetClassMap(_types.Peek());
            var instanceType = instance.GetType();
            if (!currentClassMap.ClassType.IsAssignableFrom(instanceType))
                return new ExampleClassMapPropertyDescriptor(_mappingStore, currentClassMap, instance);

            if (currentClassMap.ClassType != instanceType) //we are a subclass
                currentClassMap = _mappingStore.GetClassMap(instanceType);

            return new ClassMapPropertyDescriptor(_mappingStore, currentClassMap, instance);
        }

        public object BeginArray(object instance)
        {
            return new ArrayDescriptor((IEnumerable)instance, _types.Peek());
        }

        public IEnumerable<BsonProperty> GetProperties(object instance)
        {
            return ((IPropertyDescriptor)instance).GetProperties();
        }

        public void BeginProperty(object instance, BsonProperty property)
        {
            var value = property.Value as BsonPropertyValue;
            if (value == null)
                return;
            
            _types.Push(value.Type);
            property.Value = value.Value;
        }

        public void EndProperty(object instance, BsonProperty property)
        {
            _types.Pop();
        }

        public void EndArray(object instance)
        { }

        public void EndObject(object instance)
        { }

        public bool IsArray(object instance)
        {
            if (instance is Document)
                return false;

            return instance is IEnumerable;
        }

        public bool IsObject(object instance)
        {
            return !IsNativeToMongo(instance.GetType());
        }

        private object BeginDocument(Document document)
        {
            if (typeof(Document).IsAssignableFrom(_types.Peek()))
                return new DocumentPropertyDescriptor(document);

            var currentClassMap = _mappingStore.GetClassMap(_types.Peek());

            //var doc = (Document)instance;
            //if (doc.ContainsKey("mapreduce"))
            //    _isMapReduce = true;
            return new DocumentClassMapPropertyDescriptor(_mappingStore, currentClassMap, document);
        }

        private static bool IsNativeToMongo(Type type)
        {
            var typeCode = Type.GetTypeCode(type);

            if (typeCode != TypeCode.Object)
                return true;

            if (type == typeof(Guid))
                return true;

            if (type == typeof(Oid))
                return true;

            if (type == typeof(byte[]))
                return true;

            return false;
        }
    }
}