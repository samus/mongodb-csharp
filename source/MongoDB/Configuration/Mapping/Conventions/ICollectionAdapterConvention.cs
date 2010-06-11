using System;

using MongoDB.Configuration.CollectionAdapters;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICollectionAdapterConvention
    {
        /// <summary>
        /// Gets the collection adapter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        ICollectionAdapter GetCollectionAdapter(Type type);

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        Type GetElementType(Type type);
    }
}