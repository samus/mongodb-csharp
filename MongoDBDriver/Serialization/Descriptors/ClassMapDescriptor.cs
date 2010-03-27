using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class ClassMapDescriptor : ClassMapDescriptorBase
    {
        private readonly object _instance;
        private readonly IDictionary<string, object> _extendedProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMapDescriptor"/> class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        /// <param name="instance">The instance.</param>
        public ClassMapDescriptor(IClassMap classMap, object instance)
            : base(classMap)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _instance = instance;
            if (_classMap.HasExtendedProperties)
                _extendedProperties = (IDictionary<string, object>)_classMap.ExtendedPropertiesMap.GetValue(instance);
        }

        /// <summary>
        /// Gets the member map.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override PersistentMemberMap GetMemberMap(string name)
        {
            return _classMap.GetMemberMapFromAlias(name);
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetPropertyNames()
        {
            if (_classMap.HasId)
                yield return _classMap.IdMap.Alias;

            if (ShouldPersistDiscriminator())
                yield return _classMap.DiscriminatorAlias;

            foreach (var memberMap in _classMap.MemberMaps)
                yield return memberMap.Alias;

            if (_extendedProperties != null)
            {
                foreach (string propertyName in _extendedProperties.Keys)
                    yield return propertyName;
            }
        }

        /// <summary>
        /// Gets the property type and value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            if (_classMap.DiscriminatorAlias == name && ShouldPersistDiscriminator())
                return new KeyValuePair<Type, object>(_classMap.Discriminator.GetType(), _classMap.Discriminator);
            
            object value;

            var memberMap = _classMap.GetMemberMapFromAlias(name);
            if(memberMap != null)
                value = memberMap.GetValue(_instance);
            else if (_extendedProperties != null)
                value = _extendedProperties[name];
            else
                throw new InvalidOperationException("Attempting to get a property that does not exist.");

            var type = typeof(Document);

            if (memberMap != null)
                type = memberMap.MemberReturnType;
            else if (value != null)
                type = value.GetType();

            return new KeyValuePair<Type, object>(type, value);
        }
    }
}