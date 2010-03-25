using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Configuration.Mapping;
using MongoDB.Driver.Configuration.Mapping.Model;
using MongoDB.Driver.Serialization.Builders;

namespace MongoDB.Driver.Serialization
{
    internal class BsonObjectBuilder : IBsonObjectBuilder
    {
        private readonly Stack<Type> _types;
        private readonly IMappingStore _mappingStore;

        public BsonObjectBuilder(IMappingStore mappingStore, Type classType)
        {
            _mappingStore = mappingStore;
            _types = new Stack<Type>();
            _types.Push(classType);
        }

        public object BeginObject()
        {
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
            _types.Push(((IObjectBuilder)instance).GetPropertyType(name));
        }

        public void EndProperty(object instance, string name, object value)
        {
            _types.Pop();
            ((IObjectBuilder)instance).AddProperty(name, value);
        }
    }
}