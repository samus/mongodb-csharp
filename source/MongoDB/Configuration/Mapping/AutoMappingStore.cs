using System;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Configuration.Mapping.Auto;
using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Configuration.Mapping
{
    /// <summary>
    /// </summary>
    public class AutoMappingStore : IMappingStore
    {
        private readonly IAutoMapper _autoMapper;
        private readonly Dictionary<Type, IClassMap> _autoMaps;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly IMappingStore _wrappedMappingStore;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AutoMappingStore" /> class.
        /// </summary>
        public AutoMappingStore()
            : this(new AutoMapper())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AutoMappingStore" /> class.
        /// </summary>
        /// <param name = "profile">The profile.</param>
        public AutoMappingStore(IAutoMappingProfile profile)
            : this(new AutoMapper(profile), null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AutoMappingStore" /> class.
        /// </summary>
        /// <param name = "autoMapper">The auto mapper.</param>
        public AutoMappingStore(IAutoMapper autoMapper)
            : this(autoMapper, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AutoMappingStore" /> class.
        /// </summary>
        /// <param name = "profile">The profile.</param>
        /// <param name = "mappingStore">The mapping store.</param>
        public AutoMappingStore(IAutoMappingProfile profile, IMappingStore mappingStore)
            : this(new AutoMapper(profile), mappingStore)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AutoMappingStore" /> class.
        /// </summary>
        /// <param name = "autoMapper">The auto mapper.</param>
        /// <param name = "mappingStore">The mapping store.</param>
        public AutoMappingStore(IAutoMapper autoMapper, IMappingStore mappingStore)
        {
            if(autoMapper == null)
                throw new ArgumentNullException("autoMapper");

            _autoMapper = autoMapper;
            _autoMaps = new Dictionary<Type, IClassMap>();
            _wrappedMappingStore = mappingStore;
        }

        /// <summary>
        ///   Gets the class map for the specified class type.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <returns></returns>
        public IClassMap GetClassMap(Type classType)
        {
            try
            {
                _lock.EnterUpgradeableReadLock();

                IClassMap classMap;
                if(_autoMaps.TryGetValue(classType, out classMap))
                    return classMap;

                if(_wrappedMappingStore != null)
                {
                    classMap = _wrappedMappingStore.GetClassMap(classType);
                    if(classMap != null)
                        return classMap;
                }

                classMap = _autoMapper.CreateClassMap(classType, GetClassMap);

                try
                {
                    _lock.EnterWriteLock();

                    _autoMaps.Add(classType, classMap);

                    return classMap;
                }
                finally
                {
                    if(_lock.IsWriteLockHeld)
                        _lock.ExitWriteLock();
                }
            }
            finally
            {
                if(_lock.IsUpgradeableReadLockHeld)
                    _lock.ExitUpgradeableReadLock();
            }
        }
    }
}