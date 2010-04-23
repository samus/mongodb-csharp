using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Configuration.CollectionAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class ArrayCollectionAdapter : ICollectionAdapter
    {
        /// <summary>
        /// Adds the element to instance.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public object CreateCollection(Type elementType, object[] elements)
        {
            var array = Array.CreateInstance(elementType, elements.Length);
            elements.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Gets the elements from collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public IEnumerable GetElementsFromCollection(object collection)
        {
            return (IEnumerable)collection;
        }
    }
}
