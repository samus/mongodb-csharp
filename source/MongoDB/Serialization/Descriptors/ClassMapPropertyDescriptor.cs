using System;
using System.Collections.Generic;
using MongoDB.Configuration.Mapping.Model;
using MongoDB.Configuration.Mapping;
using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
{
    internal class ClassMapPropertyDescriptor : ClassMapPropertyDescriptorBase
    {
        private readonly object _instance;
        private readonly IDictionary<string, object> _extendedProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMapPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="mappingStore">The mapping store.</param>
        /// <param name="classMap">The class map.</param>
        /// <param name="instance">The instance.</param>
        public ClassMapPropertyDescriptor(IMappingStore mappingStore, IClassMap classMap, object instance)
            : base(mappingStore, classMap)
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
        public override IEnumerable<BsonProperty> GetProperties()
        {
            if (ClassMap.HasId)
                yield return CreateProperty(ClassMap.IdMap.Alias, ClassMap.IdMap.MemberReturnType, ClassMap.GetId(_instance), false);

            if (ShouldAddDiscriminator())
                yield return CreateProperty(ClassMap.DiscriminatorAlias, ClassMap.Discriminator.GetType(), ClassMap.Discriminator, false);

            foreach (var memberMap in ClassMap.MemberMaps)
            {
                var value = GetValue(memberMap.MemberName);
                if (!memberMap.PersistDefaultValue && object.Equals(memberMap.DefaultValue, value))
                    continue;
                yield return CreateProperty(memberMap.Alias, value);
            }

            if (_extendedProperties == null)
                yield break;

            foreach(var propertyName in _extendedProperties.Keys)
                yield return CreateProperty(propertyName, GetValue(propertyName));
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private BsonPropertyValue GetValue(string name)
        {
            if (ClassMap.DiscriminatorAlias == name && ShouldAddDiscriminator())
                return new BsonPropertyValue(ClassMap.Discriminator.GetType(), ClassMap.Discriminator, false);
            
            object value;

            var memberMap = GetAliasFromMemberName(name).MemberMap;
            if(memberMap != null)
                value = memberMap.GetValue(_instance);
            else if (_extendedProperties != null)
                value = _extendedProperties[name];
            else
                throw new InvalidOperationException("Attempting to get a property that does not exist.");

            var type = typeof(Document);
            bool isDictionary = false;
            if (memberMap != null)
            {
                type = memberMap.MemberReturnType;
                if (memberMap is CollectionMemberMap)
                    type = ((CollectionMemberMap)memberMap).ElementType;
                else if (memberMap is DictionaryMemberMap)
                {
                    type = ((DictionaryMemberMap)memberMap).ValueType;
                    isDictionary = true;
                }
            }
            else if (value != null)
                type = value.GetType();

            return new BsonPropertyValue(type, value, isDictionary);
        }
    }
}