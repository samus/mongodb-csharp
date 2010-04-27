using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Serialization.Builders
{
    internal class ArrayBuilder : IObjectBuilder
    {
        private readonly List<object> _elements;
        private readonly Type _elementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBuilder"/> class.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        public ArrayBuilder(Type elementType)
        {
            _elements = new List<object>();
            _elementType = elementType;
        }
        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddProperty(string name, object value)
        {
            _elements.Add(value);
        }

        /// <summary>
        /// Builds the object.
        /// </summary>
        /// <returns></returns>
        public object BuildObject()
        {
            if(IsDocumentArray)
            {
                var type = GetResultListType();

                return type == typeof(object) ? _elements : CreateTypedList(type);
            }

            return _elements.ToArray();
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type GetPropertyType(string name)
        {
            return _elementType;
        }

        private bool IsDocumentArray{
            get{ return _elementType == null;}
        }

        /// <summary>
        ///   Gets the type of the result list.
        /// </summary>
        /// <returns></returns>
        private Type GetResultListType()
        {
            //Todo: compare the inheritance tree up to find the most common
            if(_elements.Count == 0)
                return typeof(object);

            Type commonType = null;

            foreach(var obj in _elements)
            {
                if(obj == null)
                    continue;
                var objType = obj.GetType();
                if(commonType == null)
                    commonType = objType;
                else if(commonType != objType)
                    return typeof(object);
            }

            return commonType;
        }

        /// <summary>
        /// Creates the typed list.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private object CreateTypedList(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(type);

            var list = (IList)Activator.CreateInstance(listType);

            foreach(var obj in _elements)
                list.Add(obj);

            return list;
        }
    }
}