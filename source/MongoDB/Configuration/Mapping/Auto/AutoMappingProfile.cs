using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Attributes;
using MongoDB.Configuration.CollectionAdapters;
using MongoDB.Configuration.IdGenerators;
using MongoDB.Configuration.Mapping.Conventions;
using MongoDB.Util;
using MongoDB.Configuration.DictionaryAdapters;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMappingProfile : IAutoMappingProfile
    {
        private ConventionProfile _conventions;
        private Func<Type, bool> _isSubClass;
        private IMemberFinder _memberFinder;

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
        public Func<Type, bool> IsSubClassDelegate
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
            _memberFinder = DefaultMemberFinder.Instance;
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
            var members = (from memberInfo in _memberFinder.FindMembers(classType)
                          let att = memberInfo.GetCustomAttribute<MongoIdAttribute>(true)
                          where att != null
                          select memberInfo).ToList();

            if (members.Count > 1)
                throw new InvalidOperationException("Cannot have more than 1 member marked with a MongoId Attribute.");
            if(members.Count == 0)
                return _conventions.IdConvention.GetIdMember(classType);
            return members[0];
        }

        /// <summary>
        /// Finds the members to map for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public IEnumerable<MemberInfo> FindMembers(Type classType)
        {
            return from memberInfo in _memberFinder.FindMembers(classType)
                   let doMap = memberInfo.GetCustomAttribute<MongoIgnoreAttribute>(true) == null
                   where doMap
                   select memberInfo;
        }

        /// <summary>
        /// Gets the property name for the member.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public virtual string GetAlias(Type classType, MemberInfo member)
        {
            string alias = null;
            var att = member.GetCustomAttribute<MongoAliasAttribute>(true);
            if (att != null)
                alias = att.Name;
            if (string.IsNullOrEmpty(alias))
                alias = _conventions.AliasConvention.GetAlias(member) ?? member.Name;

            return alias;
        }

        /// <summary>
        /// Gets the collection name for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public virtual string GetCollectionName(Type classType)
        {
            return _conventions.CollectionNameConvention.GetCollectionName(classType) ?? classType.Name;
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
            return _conventions.CollectionAdapterConvention.GetCollectionAdapter(memberReturnType);
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
        public virtual object GetDefaultValue(Type classType, MemberInfo member)
        {
            object defaultValue = null;
            var att = member.GetCustomAttribute<MongoDefaultAttribute>(true);
            if (att != null)
                defaultValue = att.Value;
            return defaultValue ?? (_conventions.DefaultValueConvention.GetDefaultValue(member.GetReturnType()));
        }

        /// <summary>
        /// Gets the dictionary adadpter.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        public IDictionaryAdapter GetDictionaryAdapter(Type classType, MemberInfo member, Type memberReturnType)
        {
            return _conventions.DictionaryAdapterConvention.GetDictionaryAdapter(memberReturnType);
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
        /// Gets a value indicating whether the member should be persisted if it is null.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public bool GetPersistDefaultValue(Type classType, MemberInfo member)
        {
            var att = member.GetCustomAttribute<MongoDefaultAttribute>(true);
            if (att != null)
                return att.PersistDefaultValue;
            
            return true;
        }

        /// <summary>
        /// Indicates whether the class type is a sub class.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns>
        /// 	<c>true</c> if the classType is a sub class; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSubClass(Type classType)
        {
            return _isSubClass(classType);
        }
    }
}