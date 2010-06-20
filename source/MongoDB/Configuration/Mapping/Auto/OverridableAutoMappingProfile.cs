using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Configuration.CollectionAdapters;
using MongoDB.Configuration.DictionaryAdapters;
using MongoDB.Configuration.IdGenerators;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// </summary>
    public class OverridableAutoMappingProfile : IAutoMappingProfile
    {
        private readonly ClassOverridesMap _overrides;
        private readonly IAutoMappingProfile _profile;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AutoMappingProfile" /> class.
        /// </summary>
        /// <param name = "profile">The profile.</param>
        /// <param name = "overrides">The overrides.</param>
        public OverridableAutoMappingProfile(IAutoMappingProfile profile, ClassOverridesMap overrides)
        {
            if(overrides == null)
                throw new ArgumentNullException("overrides");
            if(profile == null)
                throw new ArgumentNullException("profile");

            _overrides = overrides;
            _profile = profile;
        }

        /// <summary>
        ///   Finds the extended properties member.
        /// </summary>
        /// <param name = "classType">Type of the class.</param>
        /// <returns></returns>
        public MemberInfo FindExtendedPropertiesMember(Type classType)
        {
            return _profile.FindExtendedPropertiesMember(classType);
        }

        /// <summary>
        ///   Gets the id member for the class type.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <returns></returns>
        public MemberInfo FindIdMember(Type classType)
        {
            return GetIdOverrideValue(
                classType,
                o => o.Member,
                m => m != null,
                _profile.FindIdMember(classType));
        }

        /// <summary>
        ///   Finds the members to map for the class type.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <returns></returns>
        public IEnumerable<MemberInfo> FindMembers(Type classType)
        {
            return _profile.FindMembers(classType)
                .Where(member => (bool)GetMemberOverrideValue(classType, member, o => !o.Ignore, v => v.HasValue, true));
        }

        /// <summary>
        ///   Gets the property name for the member.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <param name = "member">The member.</param>
        /// <returns></returns>
        public string GetAlias(Type classType, MemberInfo member)
        {
            var alias = _profile.GetAlias(classType, member);

            return GetMemberOverrideValue(classType,
                member,
                o => o.Alias,
                s => !string.IsNullOrEmpty(s),
                alias);
        }

        /// <summary>
        ///   Gets the collection name for the class type.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <returns></returns>
        public string GetCollectionName(Type classType)
        {
            return GetClassOverrideValue(classType,
                o => o.CollectionName,
                s => !string.IsNullOrEmpty(s),
                _profile.GetCollectionName(classType));
        }

        /// <summary>
        ///   Gets the type of the collection.
        /// </summary>
        /// <param name = "classType">Type of the class.</param>
        /// <param name = "member">The member.</param>
        /// <param name = "memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        public ICollectionAdapter GetCollectionAdapter(Type classType, MemberInfo member, Type memberReturnType)
        {
            return _profile.GetCollectionAdapter(classType, member, memberReturnType);
        }

        /// <summary>
        ///   Gets the type of the collection element.
        /// </summary>
        /// <param name = "classType">Type of the class.</param>
        /// <param name = "member">The member.</param>
        /// <param name = "memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        public Type GetCollectionElementType(Type classType, MemberInfo member, Type memberReturnType)
        {
            return _profile.GetCollectionElementType(classType, member, memberReturnType);
        }

        /// <summary>
        ///   Gets the default value.
        /// </summary>
        /// <param name = "classType">Type of the class.</param>
        /// <param name = "member">The member.</param>
        /// <returns></returns>
        public object GetDefaultValue(Type classType, MemberInfo member)
        {
            var defaultValue = _profile.GetDefaultValue(classType, member);

            return GetMemberOverrideValue(classType,
                member,
                o => o.DefaultValue,
                v => v != null,
                defaultValue);
        }

        /// <summary>
        ///   Gets the dictionary adadpter.
        /// </summary>
        /// <param name = "classType">Type of the class.</param>
        /// <param name = "member">The member.</param>
        /// <param name = "memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        public IDictionaryAdapter GetDictionaryAdapter(Type classType, MemberInfo member, Type memberReturnType)
        {
            return _profile.GetDictionaryAdapter(classType, member, memberReturnType);
        }

        /// <summary>
        ///   Gets the descriminator for the class type.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <returns></returns>
        public object GetDiscriminator(Type classType)
        {
            return _profile.GetDiscriminator(classType);
        }

        /// <summary>
        ///   Gets the property name of the discriminator for the class type.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <returns></returns>
        public string GetDiscriminatorAlias(Type classType)
        {
            return _profile.GetDiscriminatorAlias(classType);
        }

        /// <summary>
        ///   Gets the id generator for the member.
        /// </summary>
        /// <param name = "classType"></param>
        /// <param name = "member">The member.</param>
        /// <returns></returns>
        public IIdGenerator GetIdGenerator(Type classType, MemberInfo member)
        {
            return GetIdOverrideValue(
                classType,
                o => o.Generator,
                g => g != null,
                _profile.GetIdGenerator(classType, member));
        }

        /// <summary>
        ///   Gets the unsaved value for the id.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        /// <param name = "member">The member.</param>
        /// <returns></returns>
        public object GetIdUnsavedValue(Type classType, MemberInfo member)
        {
            return GetIdOverrideValue(
                classType,
                o => o.UnsavedValue,
                v => v != null,
                _profile.GetIdUnsavedValue(classType, member));
        }

        /// <summary>
        ///   Gets a value indicating whether the member should be persisted if it is null.
        /// </summary>
        /// <param name = "classType">Type of the class.</param>
        /// <param name = "member">The member.</param>
        /// <returns></returns>
        public bool GetPersistNull(Type classType, MemberInfo member)
        {
            return (bool)GetMemberOverrideValue(classType,
                member,
                o => o.PersistIfNull,
                v => v.HasValue,
                _profile.GetPersistNull(classType, member));
        }

        /// <summary>
        ///   Indicates whether the class type is a sub class.
        /// </summary>
        /// <param name = "classType">Type of the class.</param>
        /// <returns>
        ///   <c>true</c> if the classType is a sub class; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSubClass(Type classType)
        {
            return _profile.IsSubClass(classType);
        }

        /// <summary>
        ///   Gets the class override value.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "classType">Type of the class.</param>
        /// <param name = "overrides">The overrides.</param>
        /// <param name = "accept">The accept.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns></returns>
        private T GetClassOverrideValue<T>(Type classType, Func<ClassOverrides, T> overrides, Func<T, bool> accept, T defaultValue)
        {
            if(!_overrides.HasOverridesForType(classType))
                return defaultValue;

            var value = overrides(_overrides.GetOverridesForType(classType));
            
            return !accept(value) ? defaultValue : value;
        }

        private T GetIdOverrideValue<T>(Type classType,
           Func<IdOverrides, T> overrides,
           Func<T, bool> accept,
           T defaultValue)
        {
            if (!_overrides.HasOverridesForType(classType))
                return defaultValue;

            var idOverrides = _overrides.GetOverridesForType(classType).GetIdOverrides();
            if (idOverrides == null) 
                return defaultValue;

            var value = overrides(idOverrides);

            return !accept(value) ? defaultValue : value;
        }

        private T GetMemberOverrideValue<T>(Type classType,
            MemberInfo member,
            Func<MemberOverrides, T> overrides,
            Func<T, bool> accept,
            T defaultValue)
        {
            if(!_overrides.HasOverridesForType(classType))
                return defaultValue;

            var value = overrides(_overrides.GetOverridesForType(classType).GetOverridesFor(member));

            return !accept(value) ? defaultValue : value;
        }
    }
}