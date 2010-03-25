using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Builders
{
    internal class ArrayBuilder : IObjectBuilder
    {
        private readonly List<object> _elements;
        private readonly Type _elementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBuilder"/> class.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        public ArrayBuilder(Type elementType)
        {
            _elements = new List<object>();
            _elementType = elementType;
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
            return _elements.ToArray();
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type GetPropertyType(string name)
        {
            return _elementType;
        }
    }
}