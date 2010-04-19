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
        private List<IAutoMapper> _autoMappers;
        private IAutoMappingProfile _defaultProfile;
        private List<Type> _eagerMaps;
        private ClassOverridesMap _overrides;

        public FluentConfiguration()
        {
            _autoMappers = new List<IAutoMapper>();
            _defaultProfile = new AutoMappingProfile();
            _eagerMaps = new List<Type>();
            _overrides = new ClassOverridesMap();
        }

        public FluentConfiguration Mappings(Action<FluentMappingConfiguration> config)
        {
            var mappings = new FluentMappingConfiguration(_defaultProfile, _overrides);
            config(mappings);
            _autoMappers.AddRange(mappings.GetAutoMappers());
            _defaultProfile = mappings.DefaultProfile;
            _eagerMaps.AddRange(mappings.TypesToEagerMap);
            return this;
        }

        public IMappingStore BuildMappingStore()
        {
            IAutoMapper autoMapper;
            if (_autoMappers.Count > 0)
            {
                AggregateAutoMapper aggregate = new AggregateAutoMapper();
                foreach (var am in _autoMappers)
                    aggregate.AddAutoMapper(am);

                aggregate.AddAutoMapper(new AutoMapper(new OverridableAutoMappingProfile(_defaultProfile, _overrides)));
                autoMapper = aggregate;
            }
            else
                autoMapper = new AutoMapper(new OverridableAutoMappingProfile(_defaultProfile, _overrides));

            var store = new AutoMappingStore(autoMapper);

            foreach (var type in _eagerMaps)
                store.GetClassMap(type);

            return store;
        }

        public ISerializationFactory BuildSerializationFactory()
        {
            return new SerializationFactory(BuildMappingStore());
        }
    }
}