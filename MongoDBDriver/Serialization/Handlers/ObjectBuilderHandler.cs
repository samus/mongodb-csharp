using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Driver.Serialization.Attributes;

namespace MongoDB.Driver.Serialization.Handlers
{
    public class ObjectBuilderHandler : IBsonBuilderHandler
    {
        private readonly Type _type;
        private readonly object _instance;
        private PropertyDescriptor _currentProperty;
        private Dictionary<string,PropertyDescriptor> _propertys = new Dictionary<string, PropertyDescriptor>();

        public ObjectBuilderHandler(Type type){
            if(type == null)
                throw new ArgumentNullException("type");
            
            _type = type;
            _instance = Activator.CreateInstance(type);
            //_instance = FormatterServices.GetUninitializedObject(type);

            foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(_type)){
                var nameAttribute = (MongoNameAttribute)property.Attributes[typeof(MongoNameAttribute)];
                _propertys.Add(nameAttribute != null ? nameAttribute.Name : property.Name, property);
            }
        }

        public Type BeginProperty(string name){
            _currentProperty = _propertys[name];
            return _currentProperty.PropertyType;
        }

        public void EndProperty(object value){
            _currentProperty.SetValue(_instance,value);
        }

        public object Complete(){
            return _instance;
        }
    }
}