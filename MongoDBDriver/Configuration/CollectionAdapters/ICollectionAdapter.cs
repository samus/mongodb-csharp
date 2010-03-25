using System;
using System.Collections;

namespace MongoDB.Driver.Configuration.CollectionAdapters
{
    public interface ICollectionAdapter
    {
        /// <summary>
        /// Adds the element to instance.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        object CreateCollection(Type elementType, object[] elements);

        /// <summary>
        /// Gets the elements from collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        IEnumerable GetElementsFromCollection(object collection);
    }
}