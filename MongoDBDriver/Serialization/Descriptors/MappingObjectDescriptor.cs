using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public class MappingObjectDescriptor : IPropertyDescriptor
    {
        private readonly TypeEntry _expectedEntry;
        private readonly object _instance;
        private readonly TypeEntry _instanceEntry;
        private readonly Dictionary<string, TypeTupel> _propertys = new Dictionary<string, TypeTupel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingObjectDescriptor"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="expectedType">The type.</param>
        /// <param name="registry">The registry.</param>
        public MappingObjectDescriptor(object instance, Type instanceType, Type expectedType, TypeRegistry registry){
            if(instance == null)
                throw new ArgumentNullException("instance");
            if(registry == null)
                throw new ArgumentNullException("registry");
            _instance = instance;
            _instanceEntry = registry.GetOrCreate(instanceType);
            _expectedEntry = registry.GetOrCreate(expectedType);

            foreach(var typeProperty in _instanceEntry.Propertys)
            {
                var expectedProperty = _expectedEntry.GetProperty(typeProperty.PropertyName);

                _propertys.Add(expectedProperty.MongoName,
                    new TypeTupel
                    {
                        PropertyType = expectedProperty.PropertyType,
                        GetValue = typeProperty.GetValue
                    });
            }
        }

        class TypeTupel
        {
            public Type PropertyType;
            public TypeProperty.GetValueFunc GetValue; 
        }

        /// <summary>
        ///   Gets the property names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames(){
            foreach(var property in _propertys)
                yield return property.Key;
        }

        /// <summary>
        ///   Gets the property value.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <returns></returns>
        public KeyValuePair<Type, object> GetPropertyValue(string name){
            var property = _propertys[name];

            var value = property.GetValue(_instance);
            return new KeyValuePair<Type, object>(property.PropertyType, value);
        }
    }
}