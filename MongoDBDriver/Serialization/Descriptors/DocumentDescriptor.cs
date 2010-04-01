using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class DocumentDescriptor : IPropertyDescriptor
    {
        private readonly Document _document;

        public DocumentDescriptor(Document document)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            _document = document;
        }

        public IEnumerable<string> GetPropertyNames(){
            return _document.Keys;
        }

        public KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            var value = _document[name];
            var valueType = value == null ? typeof(Document) : value.GetType();
            return new KeyValuePair<Type, object>(valueType, value);
        }
    }
}