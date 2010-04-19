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
        private List<Type> _eagerMaps;

        internal IAutoMappingProfile DefaultProfile
        {
            get { return _defaultProfile; }
        }

        internal IEnumerable<Type> TypesToEagerMap
        {
            get { return _eagerMaps; }
        }

        public FluentMappingConfiguration(IAutoMappingProfile defaultProfile, ClassOverridesMap overrides)
        {
            if (defaultProfile == null)
                throw new ArgumentNullException("defaultProfile");
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            _autoMappers = new List<IAutoMapper>();
            _defaultProfile = defaultProfile;
            _eagerMaps = new List<Type>();
            _overrides = overrides;
        }

        public void ConfigureDefaultProfile(Action<FluentAutoMappingProfileConfiguration> config)
        {
            if(!(_defaultProfile is AutoMappingProfile))
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

        public void Map<T>()
        {
            _eagerMaps.Add(typeof(T));
        }

        public void SetDefaultProfile(IAutoMappingProfile defaultProfile)
        {
            _defaultProfile = defaultProfile;
        }

        internal IEnumerable<IAutoMapper> GetAutoMappers()
        {
            foreach (var am in _autoMappers)
                yield return am;
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