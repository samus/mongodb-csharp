using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MongoDB.Driver.Serialization
{
    public class ObjectDescriptor
    {
        public delegate object LazyPropertyDelegate();

        public bool ContainsName(object instance,string name){
            var type = instance.GetType();

            return GetPropertyDescriptor(type, name) != null;
        }

        public object GetPropteryValue(object instance,string name){
            if(instance is Document){
                return ((Document)instance)[name];
            }
            
            var type = instance.GetType();

            var proptery = GetPropertyDescriptor(type, name);

            return proptery==null ? null : proptery.GetValue(instance);
        }

        public void SetPropertyValue(object instance,string name,object value){
            if(instance is Document){
                ((Document)instance)[name] = value;
                return;
            }

            var type = instance.GetType();

            var proptery = GetPropertyDescriptor(type, name);

            proptery.SetValue(instance,value);
        }

        public void SetPropertyValueIfEmpty(object instance, string name, LazyPropertyDelegate lazyValue)
        {
            if(instance is Document){
                var document = (Document)instance;
                if(document[name]==null)
                    document[name] = lazyValue();
                return;
            }

            var type = instance.GetType();

            var proptery = GetPropertyDescriptor(type, name);

            if(proptery.GetValue(instance)==null)
                proptery.SetValue(instance, lazyValue());
        }

        public IEnumerable<string> GetPropertyNames(object instance){
            if(instance is Document){
                foreach(string key in ((Document)instance).Keys){
                    yield return key;
                }
                yield break;
            }

            var type = instance.GetType();
            foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(type)){
                yield return property.Name;
            }
        }

        private PropertyDescriptor GetPropertyDescriptor(Type type, string name){
            return TypeDescriptor.GetProperties(type).Find(name, false);
        }
    }
}