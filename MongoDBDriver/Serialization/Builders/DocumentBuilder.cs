using System;

namespace MongoDB.Driver.Serialization.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentBuilder : IObjectBuilder
    {
        private readonly Document _document = new Document();
        private string _currentKeyName;

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void EndProperty(string name,object value){
            _document.Add(name, value);
        }

        /// <summary>
        /// Completes this instance.
        /// </summary>
        /// <returns></returns>
        public object Complete(){
            if(DBRef.IsDocumentDBRef(_document))
                return DBRef.FromDocument(_document);

            return _document; 
        }

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type BeginProperty(string name){
            _currentKeyName = name;
            return typeof(Document);
        }

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="value">The value.</param>
        public void EndProperty(object value){
            _document[_currentKeyName] = value;
        }
    }
}