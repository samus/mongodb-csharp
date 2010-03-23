using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization.Descriptors;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class BsonReflectionDescriptor : IBsonObjectDescriptor
    {
        private readonly SerializationFactory _serializationFactory;
        private readonly Stack<Type> _type = new Stack<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonReflectionDescriptor"/> class.
        /// </summary>
        /// <param name="serializationFactory">The serialization factory.</param>
        /// <param name="rootType">Type of the root.</param>
        public BsonReflectionDescriptor(SerializationFactory serializationFactory,Type rootType){
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
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Begins the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object BeginArray(object instance){
            return new ObjectArrayDescriptor((IEnumerable)instance);
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames(object instance){
            return ((IPropertyDescriptor)instance).GetPropertyNames();
        }

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object BeginProperty(object instance, string name){
            var pair = ((IPropertyDescriptor)instance).GetPropertyValue(name);

            _type.Push(pair.Key);

            return pair.Value;
        }

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void EndProperty(object instance, string name, object value){
            _type.Pop();
        }

        /// <summary>
        /// Ends the array.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void EndArray(object instance){
        }

        /// <summary>
        /// Ends the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void EndObject(object obj){
        }

        /// <summary>
        /// Determines whether the specified obj is array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 	<c>true</c> if the specified obj is array; otherwise, <c>false</c>.
        /// </returns>
        public bool IsArray(object obj)
        {
            if(obj is Document)
                return false;

            return obj is IEnumerable;
        }

        /// <summary>
        /// Determines whether the specified obj is object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 	<c>true</c> if the specified obj is object; otherwise, <c>false</c>.
        /// </returns>
        public bool IsObject(object obj)
        {
            return true;
        }
    }
}