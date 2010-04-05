using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class QueriedPropertyDescriptor : IPropertyDescriptor
    {
        private readonly Document _document;
        private readonly Type _rootType;

        public QueriedPropertyDescriptor(Document document, Type rootType)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (rootType == null)
                throw new ArgumentNullException("rootType");

            _document = document;
            _rootType = rootType;
        }

        public IEnumerable<string> GetPropertyNames()
        {
            return _document.Keys;
        }

        public KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            if (name == "query")
                return new KeyValuePair<Type, object>(_rootType, _document["query"]);

            var value = _document[name];
            var type = value == null
                ? null
                : value.GetType();

            return new KeyValuePair<Type, object>(type, value);
        }
    }
}