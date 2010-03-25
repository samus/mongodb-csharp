using System;

using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Configuration.IdGenerators
{
    public interface IIdGenerator
    {
        /// <summary>
        /// Generates an id for the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="idMap">The id map.</param>
        /// <returns></returns>
        object Generate(object entity, IdMap idMap);
    }
}
