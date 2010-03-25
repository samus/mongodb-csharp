using System;
using System.Collections.Generic;
using System.Reflection;

using MongoDB.Driver;
using MongoDB.Driver.Configuration.CollectionAdapters;
using MongoDB.Driver.Configuration.Mapping.Model;
using MongoDB.Driver.Util;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMapper : IAutoMapper
    {
        private readonly Func<Type, bool> _filter;
        private readonly IAutoMappingProfile _profile;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapper"/> class.
        /// </summary>
        public AutoMapper()
            : this(null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapper"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        public AutoMapper(IAutoMappingProfile profile)
            : this(profile, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapper"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public AutoMapper(Func<Type, bool> filter)
            : this(null, filter)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapper"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="filter">The filter.</param>
        public AutoMapper(IAutoMappingProfile profile, Func<Type, bool> filter)
        {
            _filter = filter ?? new Func<Type, bool>(t => true);
            _profile = profile ?? new AutoMappingProfile();
        }

        /// <summary>
        /// Creates the class map.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <param name="classMapFinder">The class map finder.</param>
        /// <returns></returns>
        public IClassMap CreateClassMap(Type classType, Func<Type, IClassMap> classMapFinder)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");
            if (classMapFinder == null)
                throw new ArgumentNullException("classMapFinder");

            if (!_filter(classType))
                return null;

            if (_profile.IsSubClass(classType))
                return CreateSubClassMap(classType, classMapFinder);

            return CreateClassMap(classType);
        }

        /// <summary>
        /// Creates the class map.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        private ClassMap CreateClassMap(Type classType)
        {
            ClassMap classMap = new ClassMap(classType);
            classMap.CollectionName = _profile.GetCollectionName(classType);
            classMap.DiscriminatorAlias = _profile.GetDiscriminatorAlias(classType);
            if (!classType.IsInterface && !classType.IsAbstract)
                classMap.Discriminator = _profile.GetDiscriminator(classType);

            classMap.IdMap = CreateIdMap(classType);
            classMap.ExtendedPropertiesMap = CreateExtendedPropertiesMap(classType);

            foreach (MemberInfo member in _profile.FindMembers(classType))
            {
                if (classMap.HasId && classMap.IdMap.MemberName == member.Name)
                    continue;
                if (classMap.HasExtendedProperties && classMap.ExtendedPropertiesMap.MemberName == member.Name)
                    continue;

                classMap.AddMemberMap(CreateMemberMap(classType, member));
            }

            return classMap;
        }

        private SubClassMap CreateSubClassMap(Type classType, Func<Type, IClassMap> classMapFinder)
        {
            //TODO: should probably do something different to find the base type
            //mabe a convention?
            IClassMap superClassMap = classMapFinder(classType.BaseType);
            if (superClassMap == null)
                throw new InvalidOperationException(string.Format("Unable to find super class map for subclass {0}", classType));
            if (superClassMap is SubClassMap)
                throw new NotSupportedException("2-level inheritance hierarchies are not currently supported.");
            if (classType.IsInterface || classType.IsAbstract)
                throw new NotSupportedException("Only concrete classes can be subclasses.");

            SubClassMap subClassMap = new SubClassMap(classType);
            ((ClassMap)superClassMap).AddSubClassMap(subClassMap);
            subClassMap.Discriminator = _profile.GetDiscriminator(classType);

            foreach (MemberInfo member in _profile.FindMembers(classType))
            {
                if (subClassMap.HasId && subClassMap.IdMap.MemberName == member.Name)
                    continue;

                if (subClassMap.HasExtendedProperties && subClassMap.ExtendedPropertiesMap.MemberName == member.Name)
                    continue;

                if (superClassMap.GetMemberMapFromMemberName(member.Name) != null)
                    continue; //don't want to remap a member

                subClassMap.AddMemberMap(CreateMemberMap(classType, member));
            }

            return subClassMap;
        }

        private ExtendedPropertiesMap CreateExtendedPropertiesMap(Type classType)
        {
            MemberInfo extPropMember = _profile.FindExtendedPropertiesMember(classType);
            if (extPropMember == null)
                return null;

            Type memberReturnType = extPropMember.GetReturnType();

            return new ExtendedPropertiesMap(
                extPropMember.Name,
                extPropMember.GetReturnType(),
                MemberReflectionOptimizer.GetGetter(extPropMember),
                MemberReflectionOptimizer.GetSetter(extPropMember));
        }

        private IdMap CreateIdMap(Type classType)
        {
            MemberInfo idMember = _profile.FindIdMember(classType);
            if (idMember == null)
                return null;

            Type memberReturnType = idMember.GetReturnType();

            return new IdMap(
                idMember.Name,
                memberReturnType,
                MemberReflectionOptimizer.GetGetter(idMember),
                MemberReflectionOptimizer.GetSetter(idMember),
                _profile.GetIdGenerator(classType, idMember),
                _profile.GetIdUnsavedValue(classType, idMember));
        }

        private PersistentMemberMap CreateMemberMap(Type classType, MemberInfo member)
        {
            Type memberReturnType = member.GetReturnType();

            if (memberReturnType == typeof(Document))
            {
                return new DocumentMemberMap(
                    member.Name,
                    MemberReflectionOptimizer.GetGetter(member),
                    MemberReflectionOptimizer.GetSetter(member),
                    _profile.GetAlias(classType, member),
                    _profile.GetPersistNull(classType, member));
            }

            ICollectionAdapter collectionType = _profile.GetCollectionAdapter(classType, member, memberReturnType);
            if (collectionType != null)
            {
                return new CollectionMemberMap(
                    member.Name,
                    memberReturnType,
                    MemberReflectionOptimizer.GetGetter(member),
                    MemberReflectionOptimizer.GetSetter(member),
                    _profile.GetAlias(classType, member),
                    _profile.GetPersistNull(classType, member),
                    collectionType,
                    _profile.GetCollectionElementType(classType, member, memberReturnType));
            }

            //TODO: reference checking...

            return new PersistentMemberMap(
                member.Name,
                memberReturnType,
                MemberReflectionOptimizer.GetGetter(member),
                MemberReflectionOptimizer.GetSetter(member),
                _profile.GetDefaultValue(classType, member),
                _profile.GetAlias(classType, member),
                _profile.GetPersistNull(classType, member));
        }
    }
}