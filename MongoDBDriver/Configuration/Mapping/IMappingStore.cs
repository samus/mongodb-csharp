using System;

using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Configuration.Mapping
{
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