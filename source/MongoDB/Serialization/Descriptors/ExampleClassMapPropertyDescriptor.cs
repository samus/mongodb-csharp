using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Configuration.Mapping.Model;
using MongoDB.Configuration.Mapping;
using MongoDB.Bson;

namespace MongoDB.Serialization.Descriptors
{
    internal class ExampleClassMapPropertyDescriptor : ClassMapPropertyDescriptorBase
    {
        private readonly object _example;
        private readonly Type _exampleType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleClassMapPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="mappingStore">The mapping store.</param>
        /// <param name="classMap">The class map.</param>
        /// <param name="example">The example.</param>
        public ExampleClassMapPropertyDescriptor(IMappingStore mappingStore, IClassMap classMap, object example)
            : base(mappingStore, classMap)
        {
            if (example == null)
                throw new ArgumentNullException("example");

            _example = example;
            _exampleType = _example.GetType();
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<BsonProperty> GetProperties()
        {
            if (ShouldPersistDiscriminator())
                yield return CreateProperty(ClassMap.DiscriminatorAlias, ClassMap.Discriminator.GetType(), ClassMap.Discriminator, false);

            foreach (PropertyInfo propertyInfo in _exampleType.GetProperties())
            {
                var alias = GetAliasFromMemberName(propertyInfo.Name);
                var value = GetValue(propertyInfo);
                if (alias.MemberMap != null && !alias.MemberMap.PersistDefaultValue && object.Equals(alias.MemberMap.DefaultValue, value))
                    continue;

                yield return CreateProperty(alias.Alias, value);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        private BsonPropertyValue GetValue(PropertyInfo propertyInfo)
        {
            Type type = null;
            var value = propertyInfo.GetValue(_example, null);
            if (value != null && typeof(Code).IsAssignableFrom(value.GetType()))
            {
                Code code = (Code)value;
                code.Value = TranslateJavascript(code.Value);
                return new BsonPropertyValue(typeof(Code), code, false);
            }

            bool isDictionary = false;
            var memberMap = GetAliasFromMemberName(propertyInfo.Name).MemberMap;
            if (memberMap != null)
            {
                if (memberMap is CollectionMemberMap)
                    type = ((CollectionMemberMap)memberMap).ElementType;
                else if (memberMap is DictionaryMemberMap)
                {
                    type = ((DictionaryMemberMap)memberMap).ValueType;
                    isDictionary = true;
                }

                if (type == null || type == typeof(object))
                    type = memberMap.MemberReturnType;
            }
            else
                type = propertyInfo.PropertyType;

            return new BsonPropertyValue(type, value, isDictionary);
        }
    }
}