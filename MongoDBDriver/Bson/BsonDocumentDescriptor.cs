using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    public class BsonDocumentDescriptor : IBsonObjectDescriptor
    {
        public object BeginObject(object instance){
            return instance;
        }

        public IEnumerable<object> GetPropertys(object instance){
            var document = (Document)instance;
            foreach(string key in document.Keys)
                yield return new KeyValuePair<string,object>(key,document[key]);
        }

        public string GetPropertyName(object instance, object property){
            var propertyEntry = (KeyValuePair<string, object>)property;
            return propertyEntry.Key;
        }

        public object GetPropertyValue(object instance, object property){
            var propertyEntry = (KeyValuePair<string, object>)property;
            return propertyEntry.Value;
        }
       
        public void EndObject(object obj){
        }

        public bool IsArray(object obj){
            if(obj is Document)
                return false;

            return obj is IEnumerable;
        }

        public bool IsObject(object obj){
            return obj is Document;
        }
    }
}