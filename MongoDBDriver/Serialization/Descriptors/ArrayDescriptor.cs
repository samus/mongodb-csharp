using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public class ArrayDescriptor : IPropertyDescriptor
    {
        private readonly Dictionary<string,object> _items = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayDescriptor"/> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public ArrayDescriptor(IEnumerable enumerable){
            if(enumerable == null)
                throw new ArgumentNullException("enumerable");

            var i = 0;
            foreach(var item in enumerable)
                _items.Add((i++).ToString(), item);
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames(){
            return _items.Keys;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public KeyValuePair<Type, object> GetPropertyValue(string name){
            var item = _items[name];
            return new KeyValuePair<Type, object>(item.GetType(), item);
        }
    }
}