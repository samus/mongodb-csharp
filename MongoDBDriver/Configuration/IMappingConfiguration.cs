using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Gets the mapping store.
        /// </summary>
        /// <value>The mapping store.</value>
        IMappingStore MappingStore { get; }
    }
}
