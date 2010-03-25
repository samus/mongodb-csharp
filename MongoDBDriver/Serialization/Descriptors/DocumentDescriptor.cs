using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver;
using MongoDB.Driver.Configuration.Mapping.Model;

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

        public IEnumerable<string> GetPropertyNames()
        {
            foreach (var key in _document.Keys)
                yield return key;
        }

        public KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            var value = _document[name];
            return new KeyValuePair<Type, object>(value.GetType(), value);
        }
    }
}