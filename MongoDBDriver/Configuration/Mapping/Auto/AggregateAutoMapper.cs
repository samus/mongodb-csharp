using System;
using System.Collections.Generic;

using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    public class AggregateAutoMapper : IAutoMapper
    {
        private List<IAutoMapper> _autoMappers;

        public AggregateAutoMapper()
        {
            this._autoMappers = new List<IAutoMapper>();
        }

        public void AddAutoMapper(IAutoMapper autoMapper)
        {
            if (autoMapper == null)
                throw new ArgumentNullException("autoMapper");

            this._autoMappers.Add(autoMapper);
        }

        public IClassMap CreateClassMap(Type classType, Func<Type, IClassMap> classMapFinder)
        {
            IClassMap classMap;
            foreach (IAutoMapper autoMapper in _autoMappers)
            {
                classMap = autoMapper.CreateClassMap(classType, classMapFinder);
                if (classMap != null)
                    return classMap;
            }

            throw new Exception(string.Format("Unable to create map for {0}.", classType));
        }
    }
}
