using System;
using System.Collections.Generic;
using System.Collections;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class ArrayDescriptor : IPropertyDescriptor
    {
        private readonly Type _elementType;
        private readonly IEnumerable _enumerable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayDescriptor"/> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="elementType">Type of the element.</param>
        public ArrayDescriptor(IEnumerable enumerable, Type elementType)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            _elementType = elementType;
            _enumerable = enumerable;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, KeyValuePair<Type, object>>> GetProperties()
        {
            int i = 0;
            foreach (var element in _enumerable)
            {
                yield return new KeyValuePair<string, KeyValuePair<Type, object>>(i.ToString(), GetPropertyTypeAndValue(element));
                i++;
            }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private KeyValuePair<Type, object> GetPropertyTypeAndValue(object value)
        {
            var type = _elementType;
            if(type == null)
                type = value == null ? null : value.GetType();

            return new KeyValuePair<Type, object>(type, value);
        }

        
    }
}