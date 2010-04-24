using System;
using System.Reflection;

using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Configuration.Mapping.Conventions;
using MongoDB.Driver.Util;

namespace MongoDB.Driver.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMappingProfileConfiguration
    {
        private readonly AutoMappingProfile _profile;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingProfileConfiguration"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        internal AutoMappingProfileConfiguration(AutoMappingProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            _profile = profile;
        }

        /// <summary>
        /// Aliaseses the are camel cased.
        /// </summary>
        /// <returns></returns>
        public AutoMappingProfileConfiguration AliasesAreCamelCased()
        {
            _profile.Conventions.AliasConvention = new DelegateAliasConvention(m => Inflector.ToCamelCase(m.Name));
            return this;
        }

        /// <summary>
        /// Aliaseses the are.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration AliasesAre(Func<MemberInfo, string> alias)
        {
            _profile.Conventions.AliasConvention = new DelegateAliasConvention(alias);
            return this;
        }

        /// <summary>
        /// Collectionses the are named.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration CollectionsAreNamed(Func<Type, string> collectionName)
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(collectionName);
            return this;
        }

        /// <summary>
        /// Collections the names are camel cased.
        /// </summary>
        /// <returns></returns>
        public AutoMappingProfileConfiguration CollectionNamesAreCamelCased()
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(t => Inflector.ToCamelCase(t.Name));
            return this;
        }

        /// <summary>
        /// Collections the names are camel cased and plural.
        /// </summary>
        /// <returns></returns>
        public AutoMappingProfileConfiguration CollectionNamesAreCamelCasedAndPlural()
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(t => Inflector.MakePlural(Inflector.ToCamelCase(t.Name)));
            return this;
        }

        /// <summary>
        /// Conventionses the are.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration ConventionsAre(ConventionProfile conventions)
        {
            _profile.Conventions = conventions;
            return this;
        }

        /// <summary>
        /// Discriminators the aliases are.
        /// </summary>
        /// <param name="discriminatorAlias">The discriminator alias.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration DiscriminatorAliasesAre(Func<Type, string> discriminatorAlias)
        {
            _profile.Conventions.DiscriminatorAliasConvention = new DelegateDiscriminatorAliasConvention(discriminatorAlias);
            return this;
        }

        /// <summary>
        /// Discriminators the values are.
        /// </summary>
        /// <param name="discriminator">The discriminator.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration DiscriminatorValuesAre(Func<Type, object> discriminator)
        {
            _profile.Conventions.DiscriminatorConvention = new DelegateDiscriminatorConvention(discriminator);
            return this;
        }

        /// <summary>
        /// Extendeds the properties are.
        /// </summary>
        /// <param name="extendedProperty">The extended property.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration ExtendedPropertiesAre(Func<MemberInfo, bool> extendedProperty)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(extendedProperty);
            return this;
        }

        /// <summary>
        /// Extendeds the properties are.
        /// </summary>
        /// <param name="extendedProperty">The extended property.</param>
        /// <param name="memberTypes">The member types.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration ExtendedPropertiesAre(Func<MemberInfo, bool> extendedProperty, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(extendedProperty, memberTypes, bindingFlags);
            return this;
        }

        /// <summary>
        /// Extendeds the properties are named.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration ExtendedPropertiesAreNamed(string name)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(m => m.Name == name);
            return this;
        }

        /// <summary>
        /// Extendeds the properties are named.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="memberTypes">The member types.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration ExtendedPropertiesAreNamed(string name, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(m => m.Name == name, memberTypes, bindingFlags);
            return this;
        }

        /// <summary>
        /// Finds the members with.
        /// </summary>
        /// <param name="memberFinder">The member finder.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration FindMembersWith(IMemberFinder memberFinder)
        {
            _profile.MemberFinder = memberFinder;
            return this;
        }

        /// <summary>
        /// Idses the are.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration IdsAre(Func<MemberInfo, bool> id)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(id);
            return this;
        }

        /// <summary>
        /// Idses the are.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="memberTypes">The member types.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration IdsAre(Func<MemberInfo, bool> id, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(id, memberTypes, bindingFlags);
            return this;
        }

        /// <summary>
        /// Idses the are named.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration IdsAreNamed(string name)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(m => m.Name == name);
            return this;
        }

        /// <summary>
        /// Idses the are named.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="memberTypes">The member types.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration IdsAreNamed(string name, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(m => m.Name == name, memberTypes, bindingFlags);
            return this;
        }

        /// <summary>
        /// Subs the classes are.
        /// </summary>
        /// <param name="isSubClass">The is sub class.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration SubClassesAre(Func<Type, bool> isSubClass)
        {
            _profile.IsSubClassDelegate = isSubClass;
            return this;
        }

        /// <summary>
        /// Uses the collection adapter convention.
        /// </summary>
        /// <param name="collectionAdapterConvention">The collection adapter convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseCollectionAdapterConvention(ICollectionAdapterConvention collectionAdapterConvention)
        {
            _profile.Conventions.CollectionAdapterConvention = collectionAdapterConvention;
            return this;
        }

        /// <summary>
        /// Uses the collection name convention.
        /// </summary>
        /// <param name="collectionNameConvention">The collection name convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseCollectionNameConvention(ICollectionNameConvention collectionNameConvention)
        {
            _profile.Conventions.CollectionNameConvention = collectionNameConvention;
            return this;
        }

        /// <summary>
        /// Uses the default value convention.
        /// </summary>
        /// <param name="defaultValueConvention">The default value convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseDefaultValueConvention(IDefaultValueConvention defaultValueConvention)
        {
            _profile.Conventions.DefaultValueConvention = defaultValueConvention;
            return this;
        }

        /// <summary>
        /// Uses the discriminator alias convention.
        /// </summary>
        /// <param name="discriminatorAliasConvention">The discriminator alias convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseDiscriminatorAliasConvention(IDiscriminatorAliasConvention discriminatorAliasConvention)
        {
            _profile.Conventions.DiscriminatorAliasConvention = discriminatorAliasConvention;
            return this;
        }

        /// <summary>
        /// Uses the discriminator convention.
        /// </summary>
        /// <param name="discriminatorConvention">The discriminator convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseDiscriminatorConvention(IDiscriminatorConvention discriminatorConvention)
        {
            _profile.Conventions.DiscriminatorConvention = discriminatorConvention;
            return this;
        }

        /// <summary>
        /// Uses the extended properties convention.
        /// </summary>
        /// <param name="extendedPropertiesConvention">The extended properties convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseExtendedPropertiesConvention(IExtendedPropertiesConvention extendedPropertiesConvention)
        {
            _profile.Conventions.ExtendedPropertiesConvention = extendedPropertiesConvention;
            return this;
        }

        /// <summary>
        /// Uses the id convention.
        /// </summary>
        /// <param name="idConvention">The id convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseIdConvention(IIdConvention idConvention)
        {
            _profile.Conventions.IdConvention = idConvention;
            return this;
        }

        /// <summary>
        /// Uses the id generator convention.
        /// </summary>
        /// <param name="idGeneratorConvention">The id generator convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseIdGeneratorConvention(IIdGeneratorConvention idGeneratorConvention)
        {
            _profile.Conventions.IdGeneratorConvention = idGeneratorConvention;
            return this;
        }

        /// <summary>
        /// Uses the id unsaved value convention.
        /// </summary>
        /// <param name="idUnsavedValueConvention">The id unsaved value convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseIdUnsavedValueConvention(IIdUnsavedValueConvention idUnsavedValueConvention)
        {
            _profile.Conventions.IdUnsavedValueConvention = idUnsavedValueConvention;
            return this;
        }

        /// <summary>
        /// Uses the member alias convention.
        /// </summary>
        /// <param name="aliasConvention">The alias convention.</param>
        /// <returns></returns>
        public AutoMappingProfileConfiguration UseMemberAliasConvention(IAliasConvention aliasConvention)
        {
            _profile.Conventions.AliasConvention = aliasConvention;
            return this;
        }
    }
}