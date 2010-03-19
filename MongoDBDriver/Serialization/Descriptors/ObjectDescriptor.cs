using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Descriptors
{
    public class ObjectDescriptor : IObjectDescriptor2
    {
        private readonly object _instance;
        private readonly TypeEntry _entry;

        public ObjectDescriptor(object instance,TypeEntry entry){
            if(instance == null)
                throw new ArgumentNullException("instance");
            if(entry == null)
                throw new ArgumentNullException("entry");
            _instance = instance;
            _entry = entry;
        }

        public IEnumerable<object> GetPropertys(){
            foreach(var typeProperty in _entry.Propertys){
                yield return typeProperty;
            }
        }

        public string GetPropertyName(object property){
            var typeProprety = (TypeProperty)property;
            return typeProprety.MongoName;
        }

        public object GetPropertyValue(object property){
            var typeProprety = (TypeProperty)property;
            return typeProprety.GetValue(_instance);
        }
    }
}