using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Configuration.IdGenerators
{
    ///<summary>
    ///</summary>
    public class OidGenerator : IIdGenerator
    {
        /// <summary>
        /// Generates an id for the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="idMap">The id map.</param>
        /// <returns></returns>
        public object Generate(object entity, IdMap idMap)
        {
            return Oid.NewOid();
        }
    }
}