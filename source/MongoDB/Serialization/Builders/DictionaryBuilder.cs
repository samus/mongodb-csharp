using System;

namespace MongoDB.Serialization.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class DictionaryBuilder : IObjectBuilder
    {
        private readonly Document _document;
        private readonly Type _valueType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBuilder"/> class.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        public DictionaryBuilder(Type valueType)
        {
            _document = new Document();
            _valueType = valueType;
        }

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddProperty(string name, object value)
        {
            _document.Add(name, value);
        }

        /// <summary>
        /// Builds the object.
        /// </summary>
        /// <returns></returns>
        public object BuildObject()
        {
            return _document;
        }

        /// <summary>
        /// Gets the property descriptor.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public PropertyDescriptor GetPropertyDescriptor(string name)
        {
            return new PropertyDescriptor { Type = _valueType, IsDictionary = false };
        }
    }
}