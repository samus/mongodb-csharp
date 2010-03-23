using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentDescriptor : IPropertyDescriptor
    {
        private readonly Document _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDescriptor"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public DocumentDescriptor(Document document){
            if(document == null)
                throw new ArgumentNullException("document");
            _document = document;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames(){
            foreach(var key in _document.Keys)
                yield return key;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public KeyValuePair<Type, object> GetPropertyValue(string name){
            var value = _document[name];
            return new KeyValuePair<Type, object>(typeof(Document),value);
        }
    }
}