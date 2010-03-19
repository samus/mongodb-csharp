using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public class DocumentDescriptor : IObjectDescriptor2
    {
        private readonly Document _document;

        public DocumentDescriptor(Document document){
            if(document == null)
                throw new ArgumentNullException("document");
            _document = document;
        }

        public IEnumerable<object> GetPropertys(){
            foreach(string key in _document.Keys)
                yield return new KeyValuePair<string, object>(key, _document[key]);
        }

        public string GetPropertyName(object property){
            var entry = (KeyValuePair<string, object>)property;
            return Convert.ToString(entry.Key);
        }

        public object GetPropertyValue(object property){
            var entry = (KeyValuePair<string, object>)property;
            return entry.Value;
        }
    }
}