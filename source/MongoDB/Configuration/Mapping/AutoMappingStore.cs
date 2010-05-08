using System;
using System.Collections.Generic;

using MongoDB.Configuration.Mapping.Auto;
using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Configuration.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMappingStore : IMappingStore
    {
        private readonly IAutoMapper _autoMapper;
        private readonly Dictionary<Type, IClassMap> _autoMaps;
        private readonly IMappingStore _wrappedMappingStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingStore"/> class.
        /// </summary>
        public AutoMappingStore()
            : this((IAutoMapper)null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingStore"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        public AutoMappingStore(IAutoMappingProfile profile)
            : this(new AutoMapper(profile), null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingStore"/> class.
        /// </summary>
        /// <param name="autoMapper">The auto mapper.</param>
        public AutoMappingStore(IAutoMapper autoMapper)
            : this(autoMapper, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingStore"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="mappingStore">The mapping store.</param>
        public AutoMappingStore(IAutoMappingProfile profile, IMappingStore mappingStore)
            : this(new AutoMapper(profile), mappingStore)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingStore"/> class.
        /// </summary>
        /// <param name="autoMapper">The auto mapper.</param>
        /// <param name="mappingStore">The mapping store.</param>
        public AutoMappingStore(IAutoMapper autoMapper, IMappingStore mappingStore)
        {
            _autoMapper = autoMapper ?? new AutoMapper();
            _autoMaps = new Dictionary<Type, IClassMap>();
            _wrappedMappingStore = mappingStore;
        }

        /// <summary>
        /// Gets the class map for the specified class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public IClassMap GetClassMap(Type classType)
        {
            IClassMap classMap;

            if (_autoMaps.TryGetValue(classType, out classMap))
                return classMap;

            if (_wrappedMappingStore != null)
            {
                classMap = _wrappedMappingStore.GetClassMap(classType);
                if (classMap != null)
                    return classMap;
            }

            classMap = _autoMapper.CreateClassMap(classType, GetClassMap);

            _autoMaps.Add(classType, classMap);

            return classMap;
        }
    }
}