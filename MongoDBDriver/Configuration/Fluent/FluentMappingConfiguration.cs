using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Configuration.Mapping;

namespace MongoDB.Driver.Configuration.Fluent
{
    public class FluentMappingConfiguration
    {
        private List<IAutoMapper> _autoMappers;
        private IAutoMappingProfile _defaultProfile;
        private ClassOverridesMap _overrides;

        public FluentMappingConfiguration()
        {
            _autoMappers = new List<IAutoMapper>();
            _overrides = new ClassOverridesMap();
        }

        public void ConfigureDefaultProfile(Action<FluentAutoMappingProfileConfiguration> config)
        {
            _defaultProfile = new AutoMappingProfile();
            var configuration = new FluentAutoMappingProfileConfiguration((AutoMappingProfile)_defaultProfile);
            config(configuration);
        }

        public void AddCustomProfile(Func<Type, bool> match, IAutoMappingProfile profile)
        {
            AddAutoMapper(new AutoMapper(CreateOverridableProfile(profile), match));
        }

        public void AddCustomProfile(Func<Type, bool> match, Action<FluentAutoMappingProfileConfiguration> config)
        {
            var profile = new AutoMappingProfile();
            var configuration = new FluentAutoMappingProfileConfiguration(profile);
            config(configuration);
            AddCustomProfile(match, CreateOverridableProfile(profile));
        }

        public void For<T>(Action<FluentClassMapConfiguration<T>> config)
        {
            var configuration = new FluentClassMapConfiguration<T>(_overrides.GetOverridesForType(typeof(T)));
            config(configuration);
        }

        public void SetDefaultProfile(IAutoMappingProfile defaultProfile)
        {
            _defaultProfile = defaultProfile;
        }

        internal IAutoMapper BuildAutoMapper()
        {
            if (_autoMappers.Count > 0)
            {
                var aggregate = new AggregateAutoMapper();
                foreach (var am in _autoMappers)
                    aggregate.AddAutoMapper(am);
                aggregate.AddAutoMapper(new AutoMapper(CreateOverridableProfile(_defaultProfile ?? new AutoMappingProfile())));
                return aggregate;
            }
            else
                return new AutoMapper(CreateOverridableProfile(_defaultProfile ?? new AutoMappingProfile()));
        }

        private IAutoMappingProfile CreateOverridableProfile(IAutoMappingProfile profile)
        {
            return new OverridableAutoMappingProfile(profile, _overrides);
        }

        private void AddAutoMapper(IAutoMapper autoMapper)
        {
            _autoMappers.Add(autoMapper);
        }
    }
}