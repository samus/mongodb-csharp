using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Serialization.Builders
{
    public class DictionaryBuilder : IObjectBuilder
    {
        private Document _document;
        private Type _valueType;

        public DictionaryBuilder(Type valueType)
        {
            _document = new Document();
            _valueType = valueType;
        }

        public void AddProperty(string name, object value)
        {
            _document.Add(name, value);
        }

        public object BuildObject()
        {
            return _document;
        }

        public PropertyDescriptor GetPropertyDescriptor(string name)
        {
            return new PropertyDescriptor() { Type = _valueType, IsDictionary = false };
        }
    }
}