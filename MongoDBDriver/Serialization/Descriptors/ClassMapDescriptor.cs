using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class ClassMapDescriptor : IClassMapDescriptor
    {
        private readonly IClassMap _classMap;
        private readonly object _instance;
        private readonly IDictionary<string, object> _extendedProperties;

        public ClassMapDescriptor(IClassMap classMap, object instance)
        {
            _classMap = classMap;
            _instance = instance;
            if (_classMap.HasExtendedProperties)
                _extendedProperties = (IDictionary<string, object>)_classMap.ExtendedPropertiesMap.GetValue(instance);
        }

        public PersistentMemberMap GetMemberMap(string name)
        {
            return _classMap.GetMemberMapFromAlias(name);
        }

        public IEnumerable<string> GetPropertyNames()
        {
            if (_classMap.HasId)
                yield return _classMap.IdMap.Alias;

            if (_classMap.ShouldPersistDiscriminator())
                yield return _classMap.DiscriminatorAlias;

            foreach (var memberMap in _classMap.MemberMaps)
                yield return memberMap.Alias;

            if (_extendedProperties != null)
            {
                foreach (string propertyName in _extendedProperties.Keys)
                    yield return propertyName;
            }
        }

        public KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            if (_classMap.ShouldPersistDiscriminator() && _classMap.DiscriminatorAlias == name)
                return new KeyValuePair<Type, object>(_classMap.Discriminator.GetType(), _classMap.Discriminator);
            
            object value;

            var memberMap = _classMap.GetMemberMapFromAlias(name);
            if(memberMap != null)
                value = memberMap.GetValue(_instance);
            else if (_extendedProperties != null)
                value = _extendedProperties[name];
            else
                throw new InvalidOperationException("Attempting to get a property that does not exist.");

            var type = typeof(object);

            if(value != null)
                type = value.GetType();
            else if(memberMap != null)
                type = memberMap.MemberReturnType;

            return new KeyValuePair<Type, object>(type, value);
        }
    }
}