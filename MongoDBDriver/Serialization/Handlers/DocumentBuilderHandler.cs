using System;

namespace MongoDB.Driver.Serialization.Handlers
{
    public class DocumentBuilderHandler : IBsonBuilderHandler
    {
        private readonly Document _document = new Document();
        private string _currentKeyName;

        public void EndProperty(string name,object value){
            _document.Add(name, value);
        }

        public object Complete(){
            if(DBRef.IsDocumentDBRef(_document))
                return DBRef.FromDocument(_document);

            return _document; 
        }

        public Type BeginProperty(string name){
            _currentKeyName = name;
            return typeof(Document);
        }

        public void EndProperty(object value){
            _document[_currentKeyName] = value;
        }
    }
}