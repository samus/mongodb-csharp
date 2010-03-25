using System;

using MongoDB.Driver.Configuration.IdGenerators;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
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