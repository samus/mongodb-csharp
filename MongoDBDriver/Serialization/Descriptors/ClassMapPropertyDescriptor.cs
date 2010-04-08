using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class ClassMapPropertyDescriptor : ClassMapPropertyDescriptorBase
    {
        private readonly object _instance;
        private readonly IDictionary<string, object> _extendedProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMapPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        /// <param name="instance">The instance.</param>
        public ClassMapPropertyDescriptor(IClassMap classMap, object instance)
            : base(classMap)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _instance = instance;
            if (ClassMap.HasExtendedProperties)
                _extendedProperties = (IDictionary<string, object>)ClassMap.ExtendedPropertiesMap.GetValue(instance);
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetPropertyNames()
        {
            if (ClassMap.HasId)
                yield return ClassMap.IdMap.Alias;

            if (ShouldPersistDiscriminator())
                yield return ClassMap.DiscriminatorAlias;

            foreach (var memberMap in ClassMap.MemberMaps)
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
            if (ClassMap.DiscriminatorAlias == name && ShouldPersistDiscriminator())
                return new KeyValuePair<Type, object>(ClassMap.Discriminator.GetType(), ClassMap.Discriminator);
            
            object value;

            var memberMap = ClassMap.GetMemberMapFromAlias(name);
            if(memberMap != null)
                value = memberMap.GetValue(_instance);
            else if (_extendedProperties != null)
                value = _extendedProperties[name];
            else
                throw new InvalidOperationException("Attempting to get a property that does not exist.");

            var type = typeof(Document);

            if (memberMap != null)
            {
                type = memberMap.MemberReturnType;
                if (memberMap is CollectionMemberMap)
                    type = ((CollectionMemberMap)memberMap).ElementType;
            }
            else if (value != null)
                type = value.GetType();

            return new KeyValuePair<Type, object>(type, value);
        }
    }
}