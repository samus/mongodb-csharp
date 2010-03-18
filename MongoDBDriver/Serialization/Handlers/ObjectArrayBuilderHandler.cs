using System;
using System.Collections;

namespace MongoDB.Driver.Serialization.Handlers
{
    public class ObjectArrayBuilderHandler : IBsonBuilderHandler
    {
        private readonly ArrayFactory _arrayFactory = new ArrayFactory();
        private readonly Type _type;
        private readonly Type _containingType;
        private readonly IList _list;

        public ObjectArrayBuilderHandler(Type type){
            if(type == null)
                throw new ArgumentNullException("type");

            _type = type;
            _list = (IList)_arrayFactory.Create(type, out _containingType);
        }

        public object Complete(){
            return _type.IsArray ? CreateArrayFromList() : _list;
        }

        public Type BeginProperty(string name){
            return _containingType;
        }

        public void EndProperty(object value){
            _list.Add(value);
        }

        private object CreateArrayFromList(){
            var array = Array.CreateInstance(_containingType,_list.Count);

            var index = 0;
            foreach(var obj in _list){
                array.SetValue(obj,index);
                index++;
            }

            return array;
        }
    }
}