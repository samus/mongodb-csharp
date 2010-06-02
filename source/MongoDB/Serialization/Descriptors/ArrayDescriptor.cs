using System;
using System.Collections.Generic;
using System.Collections;
using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
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
        public IEnumerable<BsonProperty> GetProperties()
        {
            int i = 0;
            foreach (var element in _enumerable)
            {
                yield return new BsonProperty(i.ToString()) { Value = GetValue(element) };
                i++;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private BsonPropertyValue GetValue(object value)
        {
            var type = _elementType ?? (value == null ? null : value.GetType());

            return new BsonPropertyValue(type, value, false);
        }

        
    }
}