using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Configuration.IdGenerators
{
    /// <summary>
    /// 
    /// </summary>
    public class AssignedIdGenerator : IIdGenerator
    {
        /// <summary>
        /// Generates an id for the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="idMap">The id map.</param>
        /// <returns></returns>
        public object Generate(object entity, IdMap idMap)
        {
            var id = idMap.GetValue(entity);

            if (Equals(id, idMap.UnsavedValue))
                throw new IdGenerationException(string.Format("Ids for {0} must be manually assigned before saving.", entity.GetType()));

            return id;
        }
    }
}
