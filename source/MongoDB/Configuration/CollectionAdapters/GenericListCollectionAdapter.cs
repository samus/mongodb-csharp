using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Driver.Configuration.CollectionAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericListCollectionAdapter : ICollectionAdapter
    {
        static readonly Type OpenListType = typeof(List<>);

        /// <summary>
        /// Adds the element to instance.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public object CreateCollection(Type elementType, object[] elements)
        {
            var closedListType = OpenListType.MakeGenericType(elementType);
            var typedElements = Array.CreateInstance(elementType, elements.Length);
            Array.Copy(elements, typedElements, elements.Length);
            return Activator.CreateInstance(closedListType, typedElements);
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
