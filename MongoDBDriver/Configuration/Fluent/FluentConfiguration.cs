using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Serialization;
using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.Configuration.Fluent
{
    public class FluentConfiguration
    {
        private IAutoMapper _autoMapper;

        public FluentConfiguration Mappings(Action<FluentMappingConfiguration> config)
        {
            var mappings = new FluentMappingConfiguration();
            config(mappings);
            _autoMapper = mappings.BuildAutoMapper();
            return this;
        }

        public ISerializationFactory Build()
        {
            var mappingStore = new AutoMappingStore(_autoMapper ?? new AutoMapper());
            return new SerializationFactory(mappingStore);
        }
    }
}