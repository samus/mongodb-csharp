using System;
using System.Collections.Generic;

using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Builders
{
    internal class PolymorphicClassMapBuilder : IObjectBuilder
    {
        private IClassMap _classMap;
        private ConcreteClassMapBuilder _concreteEntityBuilder;
        private readonly Dictionary<string, object> _properties;

        public PolymorphicClassMapBuilder(IClassMap classMap)
        {
            _classMap = classMap;
            _properties = new Dictionary<string, object>();
        }

        public void AddProperty(string name, object value)
        {
            if (_concreteEntityBuilder != null)
                _concreteEntityBuilder.AddProperty(name, value);
            else if (_classMap.DiscriminatorAlias == name)
            {
                //we have found our discriminator and *can* instantiate our type
                _classMap = _classMap.GetClassMapFromDiscriminator(value);
                _concreteEntityBuilder = new ConcreteClassMapBuilder(_classMap);
                foreach (KeyValuePair<string, object> pair in _properties)
                    _concreteEntityBuilder.AddProperty(pair.Key, pair.Value);

                _properties.Clear();
            }
            else
                _properties.Add(name, value);
        }

        public object BuildObject()
        {
            if (_concreteEntityBuilder == null)
                throw new Exception("Discriminator did not show up.");

            return _concreteEntityBuilder.BuildObject();
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