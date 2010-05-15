using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Serialization.Builders
{
    internal class DictionaryBuilder : IObjectBuilder
    {
        private readonly Type _elementType;
        private readonly List<object> _elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBuilder"/> class.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        public DictionaryBuilder(Type elementType)
        {
            if(elementType == null)
                throw new ArgumentNullException("elementType");
            _elementType = elementType;
            _elements = new List<object>();
        }

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddProperty(string name, object value)
        {
            _elements.Add(value);
        }

        /// <summary>
        /// Builds the object.
        /// </summary>
        /// <returns></returns>
        public object BuildObject()
        {
            var instance = (IDictionary)Activator.CreateInstance(_elementType);
            
            foreach(KeyValueMapper keyValue in _elements)
                instance.Add(keyValue.Key, keyValue.Value);

            return instance;
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type GetPropertyType(string name)
        {
            return typeof(KeyValueMapper);
        }

        /// <summary>
        /// 
        /// </summary>
        public class KeyValueMapper
        {
            public object Key { get; set; }
            public object Value { get; set; }
        }
    }
}