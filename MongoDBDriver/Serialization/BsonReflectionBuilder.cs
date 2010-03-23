using System;
using System.Collections.Generic;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization.Builders;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class BsonReflectionBuilder : IBsonObjectBuilder
    {
        private readonly SerializationFactory _serializationFactory;
        readonly Stack<Type> _type = new Stack<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonReflectionBuilder"/> class.
        /// </summary>
        /// <param name="serializationFactory">The serialization factory.</param>
        /// <param name="rootType">Type of the root.</param>
        public BsonReflectionBuilder(SerializationFactory serializationFactory, Type rootType){
            if(serializationFactory == null)
                throw new ArgumentNullException("serializationFactory");
            if(rootType == null)
                throw new ArgumentNullException("rootType");

            _serializationFactory = serializationFactory;
            _type.Push(rootType);
        }

        /// <summary>
        /// Begins the object.
        /// </summary>
        /// <returns></returns>
        public object BeginObject()
        {
            var type = _type.Peek();

            if(type == typeof(Document))
                return new DocumentBuilder();

            return new ObjectBuilder(_serializationFactory, type);
        }

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void BeginProperty(object instance, string name)
        {
            var handler = ((IObjectBuilder)instance);

            _type.Push(handler.BeginProperty(name));
        }

        /// <summary>
        /// Begins the array.
        /// </summary>
        /// <returns></returns>
        public object BeginArray()
        {
            var type = _type.Peek();

            if(type == typeof(Document))
                return new DocumentArrayBuilder();

            return new ObjectArrayBuilder(type);
        }

        /// <summary>
        /// Ends the object.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object EndObject(object instance)
        {
            var handler = ((IObjectBuilder)instance);

            return handler.Complete();
        }

        /// <summary>
        /// Ends the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object EndArray(object instance)
        {
            var handler = ((IObjectBuilder)instance);

            return handler.Complete();
        }

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void EndProperty(object instance, string name, object value)
        {
            var handler = ((IObjectBuilder)instance);

            handler.EndProperty(value);

            _type.Pop();
        }
    }
}