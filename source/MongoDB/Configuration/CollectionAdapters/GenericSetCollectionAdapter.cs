using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Configuration.CollectionAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericSetCollectionAdapter : ICollectionAdapter
    {
        static readonly Type OpenSetType = typeof(HashSet<>);

        /// <summary>
        /// Adds the element to instance.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public object CreateCollection(Type elementType, object[] elements)
        {
            var closedSetType = OpenSetType.MakeGenericType(elementType);
            var typedElements = Array.CreateInstance(elementType, elements.Length);
            Array.Copy(elements, typedElements, elements.Length);
            return Activator.CreateInstance(closedSetType, new[] { typedElements });
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
