using System;
using System.Collections.Generic;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization.Builders;

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
                return new DocumentBuilder();

            return new ObjectBuilder(_serializationFactory, type);
        }

        public void BeginProperty(object instance, string name)
        {
            var handler = ((IObjectBuilder)instance);

            _type.Push(handler.BeginProperty(name));
        }

        public object BeginArray()
        {
            var type = _type.Peek();

            if(type == typeof(Document))
                return new DocumentArrayBuilder();

            return new ObjectArrayBuilder(type);
        }

        public object EndObject(object instance)
        {
            var handler = ((IObjectBuilder)instance);

            return handler.Complete();
        }

        public object EndArray(object instance)
        {
            var handler = ((IObjectBuilder)instance);

            return handler.Complete();
        }

        public void EndProperty(object instance, string name, object value)
        {
            var handler = ((IObjectBuilder)instance);

            handler.EndProperty(value);

            _type.Pop();
        }
    }
}