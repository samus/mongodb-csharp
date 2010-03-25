using System;

using MongoDB.Driver.Configuration.Mapping.Model;
using System.Collections.Generic;

namespace MongoDB.Driver.Serialization.Builders
{
    internal class ConcreteClassMapBuilder : IObjectBuilder
    {
        private readonly IClassMap _classMap;
        private readonly object _instance;
        private readonly IDictionary<string, object> _extendedProperties;

        public ConcreteClassMapBuilder(IClassMap classMap)
        {
            _classMap = classMap;
            _instance = classMap.CreateInstance();

            if (_classMap.HasExtendedProperties)
            {
                var extPropType = _classMap.ExtendedPropertiesMap.MemberReturnType;
                if (extPropType == typeof(IDictionary<string, object>))
                    extPropType = typeof(Dictionary<string, object>);
                _extendedProperties = (IDictionary<string, object>)Activator.CreateInstance(extPropType);
                _classMap.ExtendedPropertiesMap.SetValue(_instance, _extendedProperties);
            }
        }

        public void AddProperty(string name, object value)
        {
            var memberMap = _classMap.GetMemberMapFromAlias(name);
            if (memberMap != null)
                memberMap.SetValue(_instance, value);
            else if (_extendedProperties != null)
                _extendedProperties.Add(name, value);
        }

        public object BuildObject()
        {
            return _instance;
        }

        public Type GetPropertyType(string name)
        {
            var memberMap = _classMap.GetMemberMapFromAlias(name);
            if (memberMap == null)
                return null;

            if (memberMap is CollectionMemberMap)
                return ((CollectionMemberMap)memberMap).ElementType;

            return memberMap.MemberReturnType;
        }
    }
}