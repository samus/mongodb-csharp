using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Bson
{
    public class BsonDocumentDescriptor : IBsonObjectDescriptor
    {
        public IEnumerable<BsonObjectProperty> GetPropertys(object obj){
            var document = (Document)obj;
            foreach(string key in document.Keys){
                yield return new BsonObjectProperty(key, document[key]);
            }
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