using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    public class BsonDocumentDescriptor : IBsonObjectDescriptor
    {
        public object BeginObject(object instance){
            return instance;
        }

        public object BeginArray(object instance){
            var document = new Document();

            var i = 0;
            foreach(var item in (IEnumerable)instance)
                document.Add((i++).ToString(), item);

            return document;
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

        public void EndArray(object instance){
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