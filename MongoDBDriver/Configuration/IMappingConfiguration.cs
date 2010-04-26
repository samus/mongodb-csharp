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
        /// Builds the mapping store.
        /// </summary>
        /// <returns></returns>
        IMappingStore BuildMappingStore();

        /// <summary>
        /// Builds the serialization factory.
        /// </summary>
        /// <returns></returns>
        ISerializationFactory BuildSerializationFactory();
    }
}
