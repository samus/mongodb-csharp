using System;
using System.Reflection;

using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Configuration.Mapping.Conventions;
using MongoDB.Driver.Util;

namespace MongoDB.Driver.Configuration
{
    public class AutoMappingProfileConfiguration
    {
        private readonly AutoMappingProfile _profile;

        internal AutoMappingProfileConfiguration(AutoMappingProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            _profile = profile;
        }

        public AutoMappingProfileConfiguration AliasesAreCamelCased()
        {
            _profile.Conventions.AliasConvention = new DelegateAliasConvention(m => Inflector.ToCamelCase(m.Name));
            return this;
        }

        public AutoMappingProfileConfiguration AliasesAre(Func<MemberInfo, string> alias)
        {
            _profile.Conventions.AliasConvention = new DelegateAliasConvention(alias);
            return this;
        }

        public AutoMappingProfileConfiguration CollectionsAreNamed(Func<Type, string> collectionName)
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(collectionName);
            return this;
        }

        public AutoMappingProfileConfiguration CollectionNamesAreCamelCased()
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(t => Inflector.ToCamelCase(t.Name));
            return this;
        }

        public AutoMappingProfileConfiguration CollectionNamesAreCamelCasedAndPlural()
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(t => Inflector.MakePlural(Inflector.ToCamelCase(t.Name)));
            return this;
        }

        public AutoMappingProfileConfiguration ConventionsAre(ConventionProfile conventions)
        {
            _profile.Conventions = conventions;
            return this;
        }

        public AutoMappingProfileConfiguration DiscriminatorAliasesAre(Func<Type, string> discriminatorAlias)
        {
            _profile.Conventions.DiscriminatorAliasConvention = new DelegateDiscriminatorAliasConvention(discriminatorAlias);
            return this;
        }

        public AutoMappingProfileConfiguration DiscriminatorValuesAre(Func<Type, object> discriminator)
        {
            _profile.Conventions.DiscriminatorConvention = new DelegateDiscriminatorConvention(discriminator);
            return this;
        }

        public AutoMappingProfileConfiguration ExtendedPropertiesAre(Func<MemberInfo, bool> extendedProperty)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(extendedProperty);
            return this;
        }

        public AutoMappingProfileConfiguration ExtendedPropertiesAre(Func<MemberInfo, bool> extendedProperty, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(extendedProperty, memberTypes, bindingFlags);
            return this;
        }

        public AutoMappingProfileConfiguration ExtendedPropertiesAreNamed(string name)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(m => m.Name == name);
            return this;
        }

        public AutoMappingProfileConfiguration ExtendedPropertiesAreNamed(string name, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(m => m.Name == name, memberTypes, bindingFlags);
            return this;
        }

        public AutoMappingProfileConfiguration FindMembersWith(IMemberFinder memberFinder)
        {
            _profile.MemberFinder = memberFinder;
            return this;
        }

        public AutoMappingProfileConfiguration IdsAre(Func<MemberInfo, bool> id)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(id);
            return this;
        }

        public AutoMappingProfileConfiguration IdsAre(Func<MemberInfo, bool> id, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(id, memberTypes, bindingFlags);
            return this;
        }

        public AutoMappingProfileConfiguration IdsAreNamed(string name)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(m => m.Name == name);
            return this;
        }

        public AutoMappingProfileConfiguration IdsAreNamed(string name, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(m => m.Name == name, memberTypes, bindingFlags);
            return this;
        }

        public AutoMappingProfileConfiguration SubClassesAre(Func<Type, bool> isSubClass)
        {
            _profile.IsSubClassDelegate = isSubClass;
            return this;
        }

        public AutoMappingProfileConfiguration UseCollectionAdapterConvention(ICollectionAdapterConvention collectionAdapterConvention)
        {
            _profile.Conventions.CollectionAdapterConvention = collectionAdapterConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseCollectionNameConvention(ICollectionNameConvention collectionNameConvention)
        {
            _profile.Conventions.CollectionNameConvention = collectionNameConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseDefaultValueConvention(IDefaultValueConvention defaultValueConvention)
        {
            _profile.Conventions.DefaultValueConvention = defaultValueConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseDiscriminatorAliasConvention(IDiscriminatorAliasConvention discriminatorAliasConvention)
        {
            _profile.Conventions.DiscriminatorAliasConvention = discriminatorAliasConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseDiscriminatorConvention(IDiscriminatorConvention discriminatorConvention)
        {
            _profile.Conventions.DiscriminatorConvention = discriminatorConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseExtendedPropertiesConvention(IExtendedPropertiesConvention extendedPropertiesConvention)
        {
            _profile.Conventions.ExtendedPropertiesConvention = extendedPropertiesConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseIdConvention(IIdConvention idConvention)
        {
            _profile.Conventions.IdConvention = idConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseIdGeneratorConvention(IIdGeneratorConvention idGeneratorConvention)
        {
            _profile.Conventions.IdGeneratorConvention = idGeneratorConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseIdUnsavedValueConvention(IIdUnsavedValueConvention idUnsavedValueConvention)
        {
            _profile.Conventions.IdUnsavedValueConvention = idUnsavedValueConvention;
            return this;
        }

        public AutoMappingProfileConfiguration UseMemberAliasConvention(IAliasConvention aliasConvention)
        {
            _profile.Conventions.AliasConvention = aliasConvention;
            return this;
        }
    }
}