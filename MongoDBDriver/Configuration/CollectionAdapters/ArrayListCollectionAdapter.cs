using System;
using System.Collections;

namespace MongoDB.Driver.Configuration.CollectionAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class ArrayListCollectionAdapter : ICollectionAdapter
    {
        /// <summary>
        /// Adds the element to instance.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public object CreateCollection(Type elementType, object[] elements)
        {
            return new ArrayList(elements);
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
