using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public class ObjectDescriptor : IObjectDescriptor2
    {
        private readonly object _instance;
        private readonly TypeEntry _entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDescriptor"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="entry">The entry.</param>
        public ObjectDescriptor(object instance,TypeEntry entry){
            if(instance == null)
                throw new ArgumentNullException("instance");
            if(entry == null)
                throw new ArgumentNullException("entry");
            _instance = instance;
            _entry = entry;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames(){
            foreach(var typeProperty in _entry.Propertys)
                yield return typeProperty.MongoName;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object GetPropertyValue(string name){
            return _entry.GetProperty(name).GetValue(_instance);
        }
    }
}