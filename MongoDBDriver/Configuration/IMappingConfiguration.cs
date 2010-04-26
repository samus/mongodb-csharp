using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver.Serialization;
using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.Configuration
{
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Gets the mapping store.
        /// </summary>
        /// <value>The mapping store.</value>
        IMappingStore MappingStore { get; }
    }
}
