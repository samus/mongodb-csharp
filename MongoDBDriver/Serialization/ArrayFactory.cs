using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Driver.Serialization
{
    public class ArrayFactory
    {
        /// <summary>
        /// Creates the specified array type and returns the type of the
        /// containing objects.
        /// </summary>
        /// <param name="arrayType">Type of the array.</param>
        /// <param name="containingType">Type of the containing.</param>
        /// <returns></returns>
        /// <exception cref="MongoException"></exception>
        public object Create(Type arrayType, out Type containingType)
        {
            containingType = typeof(object);

            if(!typeof(IEnumerable).IsAssignableFrom(arrayType))
                throw new MongoException(string.Format("Array type \"{0}\" needs to implement IEnumerable", arrayType.FullName));

            try{
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
            catch(Exception exception){
                throw new MongoException(string.Format("Exception while creating array type \"{0}\"", arrayType.FullName), exception);
            }
        }
    }
}