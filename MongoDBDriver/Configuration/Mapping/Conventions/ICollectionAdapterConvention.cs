using System;

using MongoDB.Driver.Configuration.CollectionAdapters;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICollectionAdapterConvention
    {
        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        ICollectionAdapter GetCollectionType(Type type);

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        Type GetElementType(Type type);
    }
}