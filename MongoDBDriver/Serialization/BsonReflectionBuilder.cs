using System;
using System.Collections.Generic;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization.Handlers;

namespace MongoDB.Driver.Serialization
{
    public class BsonReflectionBuilder : IBsonObjectBuilder
    {
        private readonly SerializationFactory _serializationFactory;
        readonly Stack<Type> _type = new Stack<Type>();

        public BsonReflectionBuilder(SerializationFactory serializationFactory, Type rootType){
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");
            if(rootType == null)
                throw new ArgumentNullException("rootType");

            _serializationFactory = serializationFactory;
            _type.Push(rootType);
        }

        public object BeginObject()
        {
            var type = _type.Peek();

            if(type == typeof(Document))
                return new DocumentBuilderHandler();

            return new ObjectBuilderHandler(_serializationFactory, type);
        }

        public void BeginProperty(object instance, string name)
        {
            var handler = ((IBsonBuilderHandler)instance);

            _type.Push(handler.BeginProperty(name));
        }

        public object BeginArray()
        {
            var type = _type.Peek();

            if(type == typeof(Document))
                return new DocumentArrayBuilderHandler();

            return new ObjectArrayBuilderHandler(type);
        }

        public object EndObject(object instance)
        {
            var handler = ((IBsonBuilderHandler)instance);

            return handler.Complete();
        }

        public object EndArray(object instance)
        {
            var handler = ((IBsonBuilderHandler)instance);

            return handler.Complete();
        }

        public void EndProperty(object instance, string name, object value)
        {
            var handler = ((IBsonBuilderHandler)instance);

            handler.EndProperty(value);

            _type.Pop();
        }
    }
}