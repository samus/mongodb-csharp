using System;
using System.Collections.Generic;
using System.Reflection;

using MongoDB.Driver.Attributes;
using MongoDB.Driver.Configuration.CollectionAdapters;
using MongoDB.Driver.Configuration.IdGenerators;
using MongoDB.Driver.Configuration.Mapping.Conventions;
using MongoDB.Driver.Util;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    public class AutoMappingProfile : IAutoMappingProfile
    {
        private ConventionProfile _conventions;
        private Func<Type, bool> _isSubClass;
        private IMemberFinder _memberFinder;
        private readonly Dictionary<Type, ClassOverrides> _overrides;

        /// <summary>
        /// Gets or sets the conventions.
        /// </summary>
        /// <value>The conventions.</value>
        public ConventionProfile Conventions
        {
            get { return _conventions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _conventions = value;
            }
        }

        /// <summary>
        /// Gets or sets the member finder.
        /// </summary>
        /// <value>The member finder.</value>
        public IMemberFinder MemberFinder
        {
            get { return _memberFinder; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _memberFinder = value;
            }
        }

        /// <summary>
        /// Gets or sets the is sub class.
        /// </summary>
        /// <value>The is sub class.</value>
        public Func<Type, bool> IsSubClass
        {
            get { return _isSubClass; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _isSubClass = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingProfile"/> class.
        /// </summary>
        public AutoMappingProfile()
        {
            _conventions = new ConventionProfile();
            _isSubClass = t => false;
            _memberFinder = PublicMemberFinder.Instance;
            _overrides = new Dictionary<Type, ClassOverrides>();
        }

        /// <summary>
        /// Finds the extended properties member.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns></returns>
        public MemberInfo FindExtendedPropertiesMember(Type classType)
        {
            return _conventions.ExtendedPropertiesConvention.GetExtendedPropertiesMember(classType);
        }

        /// <summary>
        /// Gets the id member for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public MemberInfo FindIdMember(Type classType)
        {
            return _conventions.IdConvention.GetIdMember(classType);
        }

        /// <summary>
        /// Finds the members to map for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public IEnumerable<MemberInfo> FindMembers(Type classType)
        {
            foreach (MemberInfo memberInfo in _memberFinder.FindMembers(classType))
            {
                if (ShouldMapMember(classType, memberInfo))
                    yield return memberInfo;
            }
        }

        /// <summary>
        /// Gets the property name for the member.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public string GetAlias(Type classType, MemberInfo member)
        {
            string alias = null;
            var att = member.GetCustomAttribute<MongoNameAttribute>(true);
            if (att != null)
                alias = att.Name;
            if (string.IsNullOrEmpty(alias))
                alias = _conventions.AliasConvention.GetAlias(member) ?? member.Name;

            return GetMemberOverrideValue<string>(classType, member,
                o => o.Alias,
                s => !string.IsNullOrEmpty(s),
                alias);
        }

        /// <summary>
        /// Gets the collection name for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public string GetCollectionName(Type classType)
        {
            string collectionName = _conventions.CollectionNameConvention.GetCollectionName(classType);
            return this.GetClassOverrideValue<string>(classType,
                o => o.CollectionName,
                s => !string.IsNullOrEmpty(s),
                _conventions.CollectionNameConvention.GetCollectionName(classType) ?? classType.Name);
        }

        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        public ICollectionAdapter GetCollectionAdapter(Type classType, MemberInfo member, Type memberReturnType)
        {
            return _conventions.CollectionAdapterConvention.GetCollectionType(memberReturnType);
        }

        /// <summary>
        /// Gets the type of the collection element.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        public Type GetCollectionElementType(Type classType, MemberInfo member, Type memberReturnType)
        {
            return _conventions.CollectionAdapterConvention.GetElementType(memberReturnType);
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public object GetDefaultValue(Type classType, MemberInfo member)
        {
            object defaultValue = null;
            var att = member.GetCustomAttribute<MongoDefaultAttribute>(true);
            if (att != null)
                defaultValue = att.Value;
            if (defaultValue == null)
                defaultValue = _conventions.DefaultValueConvention.GetDefaultValue(member.GetReturnType());

            return GetMemberOverrideValue<object>(classType, member,
                o => o.DefaultValue,
                v => v != null,
                defaultValue);
        }

        /// <summary>
        /// Gets the descriminator for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public object GetDiscriminator(Type classType)
        {
            return _conventions.DiscriminatorConvention.GetDiscriminator(classType);
        }

        /// <summary>
        /// Gets the property name of the discriminator for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public string GetDiscriminatorAlias(Type classType)
        {
            return _conventions.DiscriminatorAliasConvention.GetDiscriminatorAlias(classType);
        }

        /// <summary>
        /// Gets the id generator for the member.
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public IIdGenerator GetIdGenerator(Type classType, MemberInfo member)
        {
            return _conventions.IdGeneratorConvention.GetGenerator(member.GetReturnType());
        }

        /// <summary>
        /// Gets the unsaved value for the id.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public object GetIdUnsavedValue(Type classType, MemberInfo member)
        {
            return _conventions.IdUnsavedValueConvention.GetUnsavedValue(member.GetReturnType());
        }

        /// <summary>
        /// Gets the class overrides for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public ClassOverrides GetOverridesFor(Type classType)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");

            ClassOverrides classOverrides;
            if (!this._overrides.TryGetValue(classType, out classOverrides))
                classOverrides = this._overrides[classType] = new ClassOverrides();

            return classOverrides;
        }

        /// <summary>
        /// Gets a value indicating whether the member should be persisted if it is null.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public bool GetPersistNull(Type classType, MemberInfo member)
        {
            return (bool)GetMemberOverrideValue<bool?>(classType, member,
                o => o.PersistIfNull,
                v => v.HasValue,
                false); //perhaps make this a global setting somewhere???  A convention maybe???
        }

        private T GetClassOverrideValue<T>(Type classType, Func<ClassOverrides, T> overrides, Func<T, bool> accept, T defaultValue)
        {
            ClassOverrides classOverrides;
            if (!_overrides.TryGetValue(classType, out classOverrides))
                return defaultValue;

            var value = overrides(classOverrides);
            if (!accept(value))
                return defaultValue;

            return value;
        }

        private T GetMemberOverrideValue<T>(Type classType, MemberInfo member, Func<MemberOverrides, T> overrides, Func<T, bool> accept, T defaultValue)
        {
            ClassOverrides classOverrides;
            if (!_overrides.TryGetValue(classType, out classOverrides))
                return defaultValue;

            var value = overrides(classOverrides.GetOverridesFor(member));
            if (!accept(value))
                return defaultValue;

            return value;
        }

        private bool ShouldMapMember(Type classType, MemberInfo member)
        {
            var doMap = member.GetCustomAttribute<MongoIgnoreAttribute>(true) == null;

            return (bool)GetMemberOverrideValue<bool?>(classType, member,
                o => !o.Ignore,
                v => v.HasValue,
                doMap);
        }

    }
}