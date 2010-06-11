using System;

using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Configuration.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMappingStore
    {
        /// <summary>
        /// Gets the class map.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns></returns>
        IClassMap GetClassMap(Type classType);
    }
}