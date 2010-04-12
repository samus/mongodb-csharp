using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class DocumentPropertyDescriptor : IPropertyDescriptor
    {
        private readonly Document _document;

        public DocumentPropertyDescriptor(Document document)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            _document = document;
        }

        public IEnumerable<KeyValuePair<string, KeyValuePair<Type, object>>> GetProperties()
        {
            foreach(var pair in _document)
                yield return new KeyValuePair<string, KeyValuePair<Type, object>>(pair.Key, GetPropertyTypeAndValue(pair.Value));
        }

        private KeyValuePair<Type, object> GetPropertyTypeAndValue(object value)
        {
            var valueType = value == null ? typeof(Document) : value.GetType();
            return new KeyValuePair<Type, object>(valueType, value);
        }
    }
}