using System;
using System.Collections.Generic;
using MongoDB.Driver.Configuration.Mapping.Model;
using MongoDB.Driver.Configuration.Mapping;
using System.Text;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class ClassMapPropertyDescriptorBase : IPropertyDescriptor
    {
        private readonly IMappingStore _mappingStore;
        /// <summary>
        /// 
        /// </summary>
        protected readonly IClassMap ClassMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMapPropertyDescriptorBase"/> class.
        /// </summary>
        /// <param name="mappingStore">The mapping store.</param>
        /// <param name="classMap">The class map.</param>
        protected ClassMapPropertyDescriptorBase(IMappingStore mappingStore, IClassMap classMap)
        {
            if (mappingStore == null)
                throw new ArgumentNullException("mappingStore");
            if (classMap == null)
                throw new ArgumentNullException("classMap");

            _mappingStore = mappingStore;
            ClassMap = classMap;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<BsonProperty> GetProperties();

        /// <summary>
        /// Creates the property.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected BsonProperty CreateProperty(string alias, Type valueType, object value)
        {
            return CreateProperty(alias, new BsonPropertyValue(valueType, value));
        }

        /// <summary>
        /// Creates the property.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="pair">The pair.</param>
        /// <returns></returns>
        protected BsonProperty CreateProperty(string alias, BsonPropertyValue value)
        {
            return new BsonProperty(alias) { Value = value };
        }

        /// <summary>
        /// Gets the member map from the member name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected PersistentMemberMap GetMemberMapFromMemberName(string name)
        {
            var memberMap = ClassMap.GetMemberMapFromMemberName(name);
            if (memberMap != null)
                return memberMap;

            if (!name.Contains("."))
                return null;

            var parts = name.Split('.');
            memberMap = ClassMap.GetMemberMapFromMemberName(parts[0]);
            for (int i = 1; i < parts.Length && memberMap != null; i++)
            {
                var classMap = _mappingStore.GetClassMap(memberMap.MemberReturnType);
                memberMap = classMap.GetMemberMapFromAlias(parts[i]);
            }

            return memberMap;
        }

        /// <summary>
        /// Shoulds the persist discriminator.
        /// </summary>
        /// <returns></returns>
        protected bool ShouldPersistDiscriminator()
        {
            return (ClassMap.IsPolymorphic && ClassMap.HasDiscriminator) || ClassMap.IsSubClass;
        }

        /// <summary>
        /// Gets the name of the alias from member.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected string GetAliasFromMemberName(string name)
        {
            var memberMap = ClassMap.GetMemberMapFromMemberName(name);
            if (memberMap != null)
                return memberMap.Alias;

            if (!name.Contains("."))
                return name;

            var sb = new StringBuilder();

            var parts = name.Split('.');
            memberMap = ClassMap.GetMemberMapFromMemberName(parts[0]);
            if (memberMap == null)
                return name;

            sb.Append(memberMap.Alias);
            for (int i = 1; i < parts.Length; i++)
            {
                if(memberMap != null)
                {
                    var classMap = _mappingStore.GetClassMap(memberMap.MemberReturnType);
                    memberMap = classMap.GetMemberMapFromMemberName(parts[i]);
                }

                if (memberMap == null)
                    sb.Append(".").Append(parts[i]);
                else
                    sb.Append(".").Append(memberMap.Alias);
            }

            return sb.ToString();
        }
    }
}