using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectArrayBuilder : IObjectBuilder
    {
        private readonly Type _type;
        private readonly Type _containingType;
        private readonly IList _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectArrayBuilder"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ObjectArrayBuilder(Type type){
            if(type == null)
                throw new ArgumentNullException("type");

            _type = type;
            _list = (IList)Create(type, out _containingType);
        }

        /// <summary>
        /// Completes this instance.
        /// </summary>
        /// <returns></returns>
        public object Complete(){
            return _type.IsArray ? CreateArrayFromList() : _list;
        }

        /// <summary>
        /// Begins the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type BeginProperty(string name){
            return _containingType;
        }

        /// <summary>
        /// Ends the property.
        /// </summary>
        /// <param name="value">The value.</param>
        public void EndProperty(object value){
            _list.Add(value);
        }

        /// <summary>
        /// Creates the array from list.
        /// </summary>
        /// <returns></returns>
        private object CreateArrayFromList(){
            var array = Array.CreateInstance(_containingType,_list.Count);

            var index = 0;
            foreach(var obj in _list){
                array.SetValue(obj,index);
                index++;
            }

            return array;
        }

        /// <summary>
        /// Creates the specified array type.
        /// </summary>
        /// <param name="arrayType">Type of the array.</param>
        /// <param name="containingType">Type of the containing.</param>
        /// <returns></returns>
        private static object Create(Type arrayType, out Type containingType)
        {
            containingType = typeof(object);

            if(!typeof(IEnumerable).IsAssignableFrom(arrayType))
                throw new MongoException(string.Format("Array type \"{0}\" needs to implement IEnumerable", arrayType.FullName));

            try
            {
                if(!arrayType.IsInterface)
                {
                    if(arrayType.IsArray)
                    {
                        containingType = arrayType.GetElementType();
                        return Array.CreateInstance(containingType, 0);
                    }

                    if(arrayType.IsGenericType)
                        containingType = arrayType.GetGenericArguments()[0];

                    return Activator.CreateInstance(arrayType);
                }

                if(arrayType.IsGenericType)
                {
                    var genericType = arrayType.GetGenericArguments()[0];

                    if(typeof(IEnumerable<>).MakeGenericType(genericType).IsAssignableFrom(arrayType))
                    {
                        containingType = genericType;
                        return Activator.CreateInstance(typeof(List<>).MakeGenericType(containingType));
                    }
                }

                return new List<object>();
            }
            catch(Exception exception)
            {
                throw new MongoException(string.Format("Exception while creating array type \"{0}\"", arrayType.FullName), exception);
            }
        }

    }
}