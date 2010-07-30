using System;
using System.Collections.Generic;

using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Serialization.Builders
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
                foreach (var pair in _properties)
                    _concreteEntityBuilder.AddProperty(pair.Key, pair.Value);

                _properties.Clear();
            }
            else
                _properties.Add(name, value);
        }

        public object BuildObject()
        {
            if (_concreteEntityBuilder == null)
            {
                //we'll assume that this is the root class in the hierarchy.
                _concreteEntityBuilder = new ConcreteClassMapBuilder(_classMap);
            }

            return _concreteEntityBuilder.BuildObject();
        }

        public PropertyDescriptor GetPropertyDescriptor(string name)
        {
            var memberMap = _classMap.GetMemberMapFromAlias(name);
            if (memberMap == null)
                return null;

            var type = memberMap.MemberReturnType;
            bool isDictionary = false;
            if (memberMap is CollectionMemberMap)
                type = ((CollectionMemberMap)memberMap).ElementType;
            else if (memberMap is DictionaryMemberMap)
            {
                type = ((DictionaryMemberMap)memberMap).ValueType;
                isDictionary = true;
            }

            return new PropertyDescriptor { Type = type, IsDictionary = isDictionary };
        }
    }
}