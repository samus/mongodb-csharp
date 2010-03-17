using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Handlers
{
    public class DocumentArrayBuilderHandler : IBsonBuilderHandler
    {
        private readonly List<object> _list = new List<object>();
        
        public object Compleate(){
            var type = GetResultListType();

            if(type == typeof(object))
                return _list;

            var listType = typeof(List<>).MakeGenericType(type);

            var list = (IList)Activator.CreateInstance(listType);

            foreach(var obj in _list)
                list.Add(obj);

            return list;
        }

        public Type BeginProperty(string name){
            return typeof(Document);
        }

        public void EndProperty(object value){
            _list.Add(value);
        }

        private Type GetResultListType(){
            //Todo: compare the tree up instead only to object
            if(_list.Count == 0)
                return typeof(object);

            Type commonType = null;

            foreach(var obj in _list){
                if(obj==null)
                    continue;
                var objType = obj.GetType();
                if(commonType == null)
                    commonType = objType;
                else if(commonType != objType)
                    return typeof(object);
            }

            return commonType;
        }
    }
}