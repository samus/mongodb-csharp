using System;

using MongoDB.Configuration.IdGenerators;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIdGeneratorConvention
    {
        /// <summary>
        /// Gets the generator.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IIdGenerator GetGenerator(Type type);
    }
}