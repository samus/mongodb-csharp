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

        public IEnumerable<string> GetPropertyNames(){
            foreach(var key in _document.Keys)
                yield return key;
        }

        public object GetPropertyValue(string name){
            return _document[name];
        }
    }
}