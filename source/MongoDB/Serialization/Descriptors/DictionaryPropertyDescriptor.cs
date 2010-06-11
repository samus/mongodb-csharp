using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public class DictionaryPropertyDescriptor : IPropertyDescriptor
    {
        private readonly Document _document;
        private readonly Type _valueType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="valueType">Type of the value.</param>
        public DictionaryPropertyDescriptor(Document document, Type valueType)
        {
            _document = document;
            _valueType = valueType;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BsonProperty> GetProperties()
        {
            return _document.Select(e => new BsonProperty(e.Key) { Value = new BsonPropertyValue(_valueType, e.Value, false) });
        }
    }
}