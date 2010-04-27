using System;

using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAutoMapper
    {
        /// <summary>
        /// Creates the class map.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <param name="classMapFinder">The class map finder.</param>
        /// <returns></returns>
        IClassMap CreateClassMap(Type classType, Func<Type, IClassMap> classMapFinder);
    }
}
