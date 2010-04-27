using System;
using System.Collections.Generic;

using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class AggregateAutoMapper : IAutoMapper
    {
        private readonly List<IAutoMapper> _autoMappers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateAutoMapper"/> class.
        /// </summary>
        public AggregateAutoMapper()
        {
            this._autoMappers = new List<IAutoMapper>();
        }

        /// <summary>
        /// Adds the auto mapper.
        /// </summary>
        /// <param name="autoMapper">The auto mapper.</param>
        public void AddAutoMapper(IAutoMapper autoMapper)
        {
            if (autoMapper == null)
                throw new ArgumentNullException("autoMapper");

            this._autoMappers.Add(autoMapper);
        }

        /// <summary>
        /// Creates the class map.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <param name="classMapFinder">The class map finder.</param>
        /// <returns></returns>
        public IClassMap CreateClassMap(Type classType, Func<Type, IClassMap> classMapFinder)
        {
            foreach (var autoMapper in _autoMappers)
            {
                var classMap = autoMapper.CreateClassMap(classType, classMapFinder);
                if (classMap != null)
                    return classMap;
            }

            throw new Exception(string.Format("Unable to create map for {0}.", classType));
        }
    }
}
