using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectDescriptor : IPropertyDescriptor
    {
        private readonly object _instance;
        private readonly TypeEntry _entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDescriptor"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="registry">The registry.</param>
        public ObjectDescriptor(object instance, TypeRegistry registry){
            _instance = instance;
            var type = _instance.GetType();
            _entry = registry.GetOrCreate(type);
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames(){
            return _entry.GetMongoPropertyNames();
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public KeyValuePair<Type, object> GetPropertyValue(string name){
            var property = _entry.GetPropertyFromMongoName(name);
            var value = property.GetValue(_instance);
            return new KeyValuePair<Type, object>(property.PropertyType,value);
        }
    }
}