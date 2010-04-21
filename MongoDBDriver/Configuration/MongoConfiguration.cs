using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver.Configuration.Mapping;
using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Configuration
{
    public class MongoConfiguration
    {
        private IAutoMappingProfile _defaultProfile;
        private List<Type> _eagerMapTypes;
        private ClassOverridesMap _overrides;
        private List<FilteredProfile> _profiles;

        public MongoConfiguration()
        {
            _eagerMapTypes = new List<Type>();
            _overrides = new ClassOverridesMap();
            _profiles = new List<FilteredProfile>();
        }

        public void DefaultProfile(Action<AutoMappingProfileConfiguration> config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            var dp = _defaultProfile as AutoMappingProfile;
            if (dp == null)
                dp = new AutoMappingProfile();

            config(new AutoMappingProfileConfiguration(dp));
            _defaultProfile = dp;
        }

        public void DefaultProfile(IAutoMappingProfile defaultProfile)
        {
            if (defaultProfile == null)
                throw new ArgumentNullException("defaultProfile");

            _defaultProfile = defaultProfile;
        }

        public void CustomProfile(Func<Type, bool> filter, Action<AutoMappingProfileConfiguration> config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            var p = new AutoMappingProfile();
            config(new AutoMappingProfileConfiguration(p));
            CustomProfile(filter, p);
        }

        public void CustomProfile(Func<Type, bool> filter, IAutoMappingProfile profile)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            if (profile == null)
                throw new ArgumentNullException("profile");

            _profiles.Add(new FilteredProfile { Filter = filter, Profile = profile });
        }

        public void Map<T>()
        {
            _eagerMapTypes.Add(typeof(T));
        }

        public void Map<T>(Action<ClassMapConfiguration<T>> config)
        {
            var c = new ClassMapConfiguration<T>(_overrides.GetOverridesForType(typeof(T)));
            config(c);
            Map<T>();
        }

        public IMappingStore BuildMappingStore()
        {
            IAutoMapper autoMapper;
            if (_profiles.Count > 0)
            {
                var agg = new AggregateAutoMapper();
                foreach (var p in _profiles)
                    agg.AddAutoMapper(new AutoMapper(CreateOverrideableProfile(p.Profile), p.Filter));

                agg.AddAutoMapper(new AutoMapper(CreateOverrideableProfile(_defaultProfile ?? new AutoMappingProfile())));
                autoMapper = agg;
            }
            else
                autoMapper = new AutoMapper(CreateOverrideableProfile(_defaultProfile ?? new AutoMappingProfile()));

            var store = new AutoMappingStore(autoMapper);

            foreach (var type in _eagerMapTypes)
                store.GetClassMap(type);

            return store;
        }

        public ISerializationFactory BuildSerializationFactory()
        {
            return new SerializationFactory(BuildMappingStore());
        }

        private IAutoMappingProfile CreateOverrideableProfile(IAutoMappingProfile profile)
        {
            return new OverridableAutoMappingProfile(profile, _overrides);
        }

        private class FilteredProfile
        {
            public Func<Type, bool> Filter;
            public IAutoMappingProfile Profile;
        }
    }
}