using System;
using System.Reflection;

using MongoDB.Driver.Configuration.Mapping.Auto;
using MongoDB.Driver.Configuration.Mapping.Conventions;
using MongoDB.Driver.Util;

namespace MongoDB.Driver.Configuration.Fluent
{
    public class FluentAutoMappingProfileConfiguration
    {
        private readonly AutoMappingProfile _profile;

        internal FluentAutoMappingProfileConfiguration(AutoMappingProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            _profile = profile;
        }

        public FluentAutoMappingProfileConfiguration AliasesAreCamelCased()
        {
            _profile.Conventions.AliasConvention = new DelegateAliasConvention(m => Inflector.ToCamelCase(m.Name));
            return this;
        }

        public FluentAutoMappingProfileConfiguration AliasesAre(Func<MemberInfo, string> alias)
        {
            _profile.Conventions.AliasConvention = new DelegateAliasConvention(alias);
            return this;
        }

        public FluentAutoMappingProfileConfiguration CollectionsAreNamed(Func<Type, string> collectionName)
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(collectionName);
            return this;
        }

        public FluentAutoMappingProfileConfiguration CollectionNamesAreCamelCased()
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(t => Inflector.ToCamelCase(t.Name));
            return this;
        }

        public FluentAutoMappingProfileConfiguration CollectionNamesAreCamelCasedAndPlural()
        {
            _profile.Conventions.CollectionNameConvention = new DelegateCollectionNameConvention(t => Inflector.MakePlural(Inflector.ToCamelCase(t.Name)));
            return this;
        }

        public FluentAutoMappingProfileConfiguration ConventionsAre(ConventionProfile conventions)
        {
            _profile.Conventions = conventions;
            return this;
        }

        public FluentAutoMappingProfileConfiguration DiscriminatorAliasesAre(Func<Type, string> discriminatorAlias)
        {
            _profile.Conventions.DiscriminatorAliasConvention = new DelegateDiscriminatorAliasConvention(discriminatorAlias);
            return this;
        }

        public FluentAutoMappingProfileConfiguration DiscriminatorValuesAre(Func<Type, object> discriminator)
        {
            _profile.Conventions.DiscriminatorConvention = new DelegateDiscriminatorConvention(discriminator);
            return this;
        }

        public FluentAutoMappingProfileConfiguration ExtendedPropertiesAre(Func<MemberInfo, bool> extendedProperty)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(extendedProperty);
            return this;
        }

        public FluentAutoMappingProfileConfiguration ExtendedPropertiesAre(Func<MemberInfo, bool> extendedProperty, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(extendedProperty, memberTypes, bindingFlags);
            return this;
        }

        public FluentAutoMappingProfileConfiguration ExtendedPropertiesAreNamed(string name)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(m => m.Name == name);
            return this;
        }

        public FluentAutoMappingProfileConfiguration ExtendedPropertiesAreNamed(string name, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.ExtendedPropertiesConvention = new DelegateExtendedPropertiesConvention(m => m.Name == name, memberTypes, bindingFlags);
            return this;
        }

        public FluentAutoMappingProfileConfiguration FindMembersWith(IMemberFinder memberFinder)
        {
            _profile.MemberFinder = memberFinder;
            return this;
        }

        public FluentAutoMappingProfileConfiguration IdsAre(Func<MemberInfo, bool> id)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(id);
            return this;
        }

        public FluentAutoMappingProfileConfiguration IdsAre(Func<MemberInfo, bool> id, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(id, memberTypes, bindingFlags);
            return this;
        }

        public FluentAutoMappingProfileConfiguration IdsAreNamed(string name)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(m => m.Name == name);
            return this;
        }

        public FluentAutoMappingProfileConfiguration IdsAreNamed(string name, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _profile.Conventions.IdConvention = new DelegateIdConvention(m => m.Name == name, memberTypes, bindingFlags);
            return this;
        }

        public FluentAutoMappingProfileConfiguration SubClassesAre(Func<Type, bool> isSubClass)
        {
            _profile.IsSubClassDelegate = isSubClass;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseCollectionAdapterConvention(ICollectionAdapterConvention collectionAdapterConvention)
        {
            _profile.Conventions.CollectionAdapterConvention = collectionAdapterConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseCollectionNameConvention(ICollectionNameConvention collectionNameConvention)
        {
            _profile.Conventions.CollectionNameConvention = collectionNameConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseDefaultValueConvention(IDefaultValueConvention defaultValueConvention)
        {
            _profile.Conventions.DefaultValueConvention = defaultValueConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseDiscriminatorAliasConvention(IDiscriminatorAliasConvention discriminatorAliasConvention)
        {
            _profile.Conventions.DiscriminatorAliasConvention = discriminatorAliasConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseDiscriminatorConvention(IDiscriminatorConvention discriminatorConvention)
        {
            _profile.Conventions.DiscriminatorConvention = discriminatorConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseExtendedPropertiesConvention(IExtendedPropertiesConvention extendedPropertiesConvention)
        {
            _profile.Conventions.ExtendedPropertiesConvention = extendedPropertiesConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseIdConvention(IIdConvention idConvention)
        {
            _profile.Conventions.IdConvention = idConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseIdGeneratorConvention(IIdGeneratorConvention idGeneratorConvention)
        {
            _profile.Conventions.IdGeneratorConvention = idGeneratorConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseIdUnsavedValueConvention(IIdUnsavedValueConvention idUnsavedValueConvention)
        {
            _profile.Conventions.IdUnsavedValueConvention = idUnsavedValueConvention;
            return this;
        }

        public FluentAutoMappingProfileConfiguration UseMemberAliasConvention(IAliasConvention aliasConvention)
        {
            _profile.Conventions.AliasConvention = aliasConvention;
            return this;
        }
    }
}