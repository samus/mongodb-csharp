using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class ExampleClassMapDescriptor : ClassMapDescriptorBase
    {
        private readonly object _example;
        private readonly Type _exampleType;

        public ExampleClassMapDescriptor(IClassMap classMap, object example)
            : base(classMap)
        {
            if (example == null)
                throw new ArgumentNullException("example");

            _example = example;
            _exampleType = _example.GetType();
        }

        public override PersistentMemberMap GetMemberMap(string name)
        {
            return ClassMap.GetMemberMapFromAlias(name);
        }

        public override IEnumerable<string> GetPropertyNames()
        {
            if (ShouldPersistDiscriminator())
                yield return ClassMap.DiscriminatorAlias;

            PersistentMemberMap memberMap;
            foreach (PropertyInfo propertyInfo in _exampleType.GetProperties())
            {
                memberMap = ClassMap.GetMemberMapFromMemberName(propertyInfo.Name) as PersistentMemberMap;
                if (memberMap == null)
                    yield return propertyInfo.Name; //if it isn't mapped, we'll persist it anyways...
                else
                    yield return memberMap.Alias;
            }
        }

        public override KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            if (ClassMap.DiscriminatorAlias == name && ShouldPersistDiscriminator())
                return new KeyValuePair<Type, object>(ClassMap.Discriminator.GetType(), ClassMap.Discriminator);

            Type type;
            object value;
            PropertyInfo propInfo;

            var memberMap = ClassMap.GetMemberMapFromAlias(name);
            if (memberMap == null)
                propInfo = _exampleType.GetProperty(name);
            else
                propInfo = _exampleType.GetProperty(memberMap.MemberName);

            value = propInfo.GetValue(_example, null);
            if (value is Document)
                type = typeof(Document);
            else if (memberMap != null)
                type = memberMap.MemberReturnType;
            else
                type = propInfo.PropertyType;

            return new KeyValuePair<Type, object>(type, value);
        }
    }
}