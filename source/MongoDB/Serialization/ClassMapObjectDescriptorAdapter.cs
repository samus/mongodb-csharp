using System.Collections.Generic;
using MongoDB.Configuration.Mapping.Model;

namespace MongoDB.Serialization
{
    internal class ClassMapObjectDescriptorAdapter : IObjectDescriptor
    {
        private readonly IClassMap _classMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMapObjectDescriptorAdapter"/> class.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        public ClassMapObjectDescriptorAdapter(IClassMap classMap)
        {
            _classMap = classMap;
        }

        /// <summary>
        /// Generates the id.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object GenerateId(object instance){
            return !_classMap.HasId ? null : _classMap.IdMap.Generate(instance);
        }

        /// <summary>
        /// Gets the mongo property names.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IEnumerable<string> GetMongoPropertyNames(object instance)
        {
            if (_classMap.HasId)
                yield return _classMap.IdMap.Alias;

            if (_classMap.IsPolymorphic && _classMap.HasDiscriminator)
                yield return _classMap.DiscriminatorAlias;

            foreach (var memberMap in _classMap.MemberMaps)
                yield return memberMap.Alias;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <returns></returns>
        public object GetPropertyValue(object instance, string mongoName)
        {
            //not sure if this is necessary...
            //if (_classMap.HasDiscriminator && _classMap.DiscriminatorAlias == mongoName)
            //    return _classMap.Discriminator;

            if (!_classMap.HasId && mongoName == "_id")
                return null;

            return _classMap.GetMemberMapFromAlias(mongoName).GetValue(instance);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mongoName">Name of the mongo.</param>
        /// <param name="value">The value.</param>
        public void SetPropertyValue(object instance, string mongoName, object value)
        {
            if (!_classMap.HasId && mongoName == "_id") //there is nothing for us to set and we'll let the database do the id generation...
                return;

            _classMap.GetMemberMapFromAlias(mongoName).SetValue(instance, value);
        }
    }
}