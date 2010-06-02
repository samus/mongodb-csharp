using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Configuration.Mapping;
using MongoDB.Serialization.Builders;

namespace MongoDB.Serialization
{
    internal class BsonClassMapBuilder : IBsonObjectBuilder
    {
        private bool _isDictionary;
        private readonly Stack<Type> _types;
        private readonly IMappingStore _mappingStore;

        public BsonClassMapBuilder(IMappingStore mappingStore, Type classType)
        {
            _mappingStore = mappingStore;
            _types = new Stack<Type>();
            _types.Push(classType);
        }

        public object BeginObject()
        {
             if (_isDictionary)
            {
                 _isDictionary = false;
                 return new DictionaryBuilder(_types.Peek());
            }

            if (_types.Peek() == null || _types.Peek() == typeof(Document))
                return new DocumentBuilder();

            var classMap = _mappingStore.GetClassMap(_types.Peek());
            if (classMap.IsPolymorphic)
            {
                //until we have the discriminator, we can't instantiate the type.
                return new PolymorphicClassMapBuilder(classMap);
            }

            return new ConcreteClassMapBuilder(classMap);
        }

        public object EndObject(object instance)
        {
            return ((IObjectBuilder)instance).BuildObject();
        }

        public object BeginArray()
        {
            return new ArrayBuilder(_types.Peek());
        }

        public object EndArray(object instance)
        {
            return ((IObjectBuilder)instance).BuildObject();
        }

        public void BeginProperty(object instance, string name)
        {
            var propDescriptor = ((IObjectBuilder)instance).GetPropertyDescriptor(name);
            if (propDescriptor == null)
                _types.Push(null);
            else
            {
                _types.Push(propDescriptor.Type);
                _isDictionary = propDescriptor.IsDictionary;
            }
        }

        public void EndProperty(object instance, string name, object value)
        {
            _types.Pop();
            ((IObjectBuilder)instance).AddProperty(name, value);
        }
    }
}