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
        private readonly ArrayFactory _arrayFactory;

        public ReflectionBuilder()
        {
            _type.Push(typeof(T));
            _arrayFactory = new ArrayFactory();
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

            if(instance is Document)
            {
                var document = (Document)instance;
                if(DBRef.IsDocumentDBRef(document))
                    return DBRef.FromDocument(document);
            }
            
            return instance;
        }

        public object BeginArray()
        {
            _arrayMode.Push(true);
            var type = _type.Peek();

            if(type == typeof(Document)){
                _type.Push(typeof(Document));
                return new Document();
            }

            Type containingType;
            var instance = _arrayFactory.Create(type, out containingType);
            _type.Push(containingType);
            
            return instance;
        }

        public object EndArray(object instance)
        {
            _type.Pop();
            _arrayMode.Pop();

            if(instance is Document){
                //Todo: may there is a better way to handle that
                return ConvertToArray((Document)instance);
            }

            return instance;
        }

        public void BeginProperty(object instance, string name){
            if(IsInArrayMode || instance is Document)
                return;

            var type = instance.GetType();

            var property = TypeDescriptor.GetProperties(type).Find(name, true);

            _property.Push(property);
            _type.Push(property.PropertyType);

        }

        public void EndProperty(object instance, string name, object value)
        {
            if(instance is Document){
                var document = (Document)instance;
                document.Add(name, value);
                return;
            }

            if(IsInArrayMode)
            {
                ((IList)instance).Add(value);
            }
            else
            {
                var proprety = _property.Pop();

                proprety.SetValue(instance, value);

                _type.Pop();
            }
        }

        private bool IsInArrayMode{
            get { return _arrayMode.Count == 0 || _arrayMode.Peek(); }
        }

        /// <summary>
        /// Gets the type for IEnumerable.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private Type GetTypeForIEnumerable(Document doc)
        {
            if(doc.Keys.Count < 1)
                return typeof(Object);
            Type comp = null;
            foreach(String key in doc.Keys)
            {
                var obj = doc[key];
                var test = obj.GetType();
                if(comp == null)
                    comp = test;
                else if(comp != test)
                    return typeof(Object);
            }
            return comp;
        }

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private IEnumerable ConvertToArray(Document doc)
        {
            var genericListType = typeof(List<>);
            var arrayType = GetTypeForIEnumerable(doc);
            var listType = genericListType.MakeGenericType(arrayType);

            var list = (IList)Activator.CreateInstance(listType);

            foreach(String key in doc.Keys)
                list.Add(doc[key]);

            return list;
        }

    }
}