using System;
using System.Collections.Generic;
using System.Collections;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class ArrayDescriptor : IPropertyDescriptor
    {
        private readonly Dictionary<string, object> _items = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayDescriptor"/> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public ArrayDescriptor(IEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            var i = 0;
            foreach (var item in enumerable)
                _items.Add((i++).ToString(), item);
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames()
        {
            return _items.Keys;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            var value = _items[name];
            var type = value == null ? typeof(Document) : value.GetType();
            return new KeyValuePair<Type, object>(type, value);
        }
    }
}