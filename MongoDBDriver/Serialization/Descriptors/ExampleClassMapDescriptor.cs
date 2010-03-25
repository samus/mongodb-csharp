using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal class ExampleClassMapDescriptor : IClassMapDescriptor
    {
        private readonly IClassMap _classMap;
        private readonly object _example;
        private readonly Type _exampleType;

        public ExampleClassMapDescriptor(IClassMap classMap, object example)
        {
            if (classMap == null)
                throw new ArgumentNullException("classMap");
            if (example == null)
                throw new ArgumentNullException("example");
            _classMap = classMap;
            _example = example;
            _exampleType = _example.GetType();
        }

        public PersistentMemberMap GetMemberMap(string name)
        {
            return _classMap.GetMemberMapFromAlias(name);
        }

        public IEnumerable<string> GetPropertyNames()
        {
            if (_classMap.ShouldPersistDiscriminator())
                yield return _classMap.DiscriminatorAlias;

            PersistentMemberMap memberMap;
            foreach (PropertyInfo propertyInfo in _exampleType.GetProperties())
            {
                memberMap = _classMap.GetMemberMapFromMemberName(propertyInfo.Name) as PersistentMemberMap;
                if (memberMap == null)
                    yield return propertyInfo.Name; //if it isn't mapped, we'll persist it anyways...
                else
                    yield return memberMap.Alias;
            }
        }

        public KeyValuePair<Type, object> GetPropertyTypeAndValue(string name)
        {
            if (_classMap.ShouldPersistDiscriminator() && _classMap.DiscriminatorAlias == name)
                return new KeyValuePair<Type, object>(_classMap.Discriminator.GetType(), _classMap.Discriminator);

            var memberMap = _classMap.GetMemberMapFromAlias(name);
            if (memberMap == null) //if it isn't mapped, return it as is...
            {
                var propInfo = _exampleType.GetProperty(name);
                return new KeyValuePair<Type, object>(propInfo.PropertyType, propInfo.GetValue(_example, null));
            }

            var value = _exampleType.GetProperty(memberMap.MemberName).GetValue(_example, null);
            if (value is Document)
                return new KeyValuePair<Type, object>(typeof(Document), value);

            return new KeyValuePair<Type, object>(memberMap.MemberReturnType, value);
        }
    }
}