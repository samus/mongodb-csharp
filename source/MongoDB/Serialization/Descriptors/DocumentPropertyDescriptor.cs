using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
{
    internal class DocumentPropertyDescriptor : IPropertyDescriptor
    {
        private readonly Document _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public DocumentPropertyDescriptor(Document document)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            _document = document;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BsonProperty> GetProperties()
        {
            return _document.Select(pair => new BsonProperty(pair.Key)
            {
                Value = GetValue(pair.Value)
            });
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private BsonPropertyValue GetValue(object value)
        {
            var valueType = value == null ? typeof(Document) : value.GetType();
            return new BsonPropertyValue(valueType, value);
        }
    }
}