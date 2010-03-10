using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization
{
    public class ReflectionBuilder<T> : IBsonObjectBuilder
    {
        readonly Stack<Type> _type = new Stack<Type>();
        readonly Stack<PropertyDescriptor> _property = new Stack<PropertyDescriptor>();
        readonly Stack<bool> _arrayMode = new Stack<bool>();

        public ReflectionBuilder()
        {
            _type.Push(typeof(T));
        }

        public object BeginObject()
        {
            _arrayMode.Push(false);
            var type = _type.Peek();
            return Activator.CreateInstance(type);
            //return FormatterServices.GetUninitializedObject(type);
        }

        public object EndObject(object instance)
        {
            _arrayMode.Pop();
            return instance;
        }

        public object BeginArray()
        {
            _arrayMode.Push(true);
            var type = _type.Peek();

            if(!type.IsInterface)
                return Activator.CreateInstance(type);

            if(type.IsGenericType)
            {
                var arrayType = type.GetGenericArguments()[0];
                if(typeof(IEnumerable<>).MakeGenericType(arrayType).IsAssignableFrom(type))
                {
                    _type.Push(arrayType);
                    return Activator.CreateInstance(typeof(List<>).MakeGenericType(arrayType));
                }
            }
            
            if(typeof(IEnumerable).IsAssignableFrom(type))
                return new List<object>();
            
            throw new Exception();
        }

        public object EndArray(object instance)
        {
            _type.Pop();
            _arrayMode.Pop();
            return instance;
        }

        public void BeginProperty(object instance, string name){
            if(_arrayMode.Peek()||instance is Document)
                return;

            var type = instance.GetType();

            var property = TypeDescriptor.GetProperties(type).Find(name, true);

            _property.Push(property);
            _type.Push(property.PropertyType);

        }

        public void EndProperty(object instance, string name, object value)
        {
            if(_arrayMode.Peek())
            {
                ((IList)instance).Add(value);
            }
            else
            {
                if(instance is Document){
                    var document = (Document)instance;
                    document.Add(name, value);
                }else{
                    var proprety = _property.Pop();

                    proprety.SetValue(instance, value);

                    _type.Pop();
                }
            }
        }
    }
}