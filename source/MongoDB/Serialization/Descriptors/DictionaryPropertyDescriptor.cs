using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
{
    public class DictionaryPropertyDescriptor : IPropertyDescriptor
    {
        private readonly Document _document;
        private readonly Type _valueType;

        public DictionaryPropertyDescriptor(Document document, Type valueType)
        {
            _document = document;
            _valueType = valueType;
        }

        public IEnumerable<BsonProperty> GetProperties()
        {
            return _document.Select(e => new BsonProperty(e.Key) { Value = new BsonPropertyValue(_valueType, e.Value, false) });
        }
    }
}