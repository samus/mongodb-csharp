using System;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICollectionNameConvention
    {
        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        string GetCollectionName(Type classType);
    }
}