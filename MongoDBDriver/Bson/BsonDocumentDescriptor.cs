using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    public class BsonDocumentDescriptor : IBsonObjectDescriptor
    {
        public object BeginObject(object instance){
            return instance;
        }

        public IEnumerable<string> GetPropertyNames(object instance){
            var document = (Document)instance;
            foreach(var key in document.Keys)
                yield return key;
        }

        public object BeginProperty(object instance, string name){
            var document = (Document)instance;
            return document[name];
        }

        public void EndProperty(object instance, string name, object value){
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