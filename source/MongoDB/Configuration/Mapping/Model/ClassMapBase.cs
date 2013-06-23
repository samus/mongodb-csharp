﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Configuration.Mapping.Util;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// </summary>
    public abstract class ClassMapBase : IClassMap
    {
        private readonly List<PersistentMemberMap> _memberMaps;
        private readonly List<SubClassMap> _subClassMaps;
        private Func<object> _creator;
        private readonly bool _hasProtectedOrPublicConstructor;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ClassMapBase" /> class.
        /// </summary>
        /// <param name = "classType">Type of the entity.</param>
        protected ClassMapBase(Type classType)
        {
            if(classType == null)
                throw new ArgumentNullException("classType");

            ClassType = classType;
            _memberMaps = new List<PersistentMemberMap>();
            _subClassMaps = new List<SubClassMap>();
            _hasProtectedOrPublicConstructor = ClassType.GetConstructors(BindingFlags.Instance | 
                BindingFlags.Public | 
                BindingFlags.NonPublic)
                .Any(c => !c.IsPrivate);
        }

        /// <summary>
        ///   Gets the type of class to which this map pertains.
        /// </summary>
        /// <value>The type of the class.</value>
        public Type ClassType { get; private set; }

        /// <summary>
        ///   Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public abstract string CollectionName { get; internal set; }

        /// <summary>
        ///   Gets the discriminator.
        /// </summary>
        /// <value>The discriminator.</value>
        public object Discriminator { get; internal set; }

        /// <summary>
        ///   Gets the alias used to store the discriminator.
        /// </summary>
        /// <value>The discriminator alias.</value>
        public abstract string DiscriminatorAlias { get; internal set; }

        /// <summary>
        ///   Gets the extended properties map.
        /// </summary>
        /// <value>The extended properties map.</value>
        public abstract ExtendedPropertiesMap ExtendedPropertiesMap { get; internal set; }

        /// <summary>
        ///   Gets a value indicating whether this instance has discriminator.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has discriminator; otherwise, <c>false</c>.
        /// </value>
        public bool HasDiscriminator
        {
            get { return Discriminator != null; }
        }

        /// <summary>
        ///   Gets a value indicating whether the class map has extended properties.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the class map has extended properties; otherwise, <c>false</c>.
        /// </value>
        public bool HasExtendedProperties
        {
            get { return ExtendedPropertiesMap != null; }
        }

        /// <summary>
        ///   Gets a value indicating whether the class map has an id.
        /// </summary>
        /// <value><c>true</c> if the class map has an id; otherwise, <c>false</c>.</value>
        public virtual bool HasId
        {
            get { return IdMap != null; }
        }

        /// <summary>
        ///   Gets the id map.
        /// </summary>
        /// <value>The id map.</value>
        public abstract IdMap IdMap { get; internal set; }

        /// <summary>
        ///   Gets a value indicating whether this class map is polymorphic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this class map is polymorphic; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsPolymorphic
        {
            get { return _subClassMaps.Count > 0; }
        }

        /// <summary>
        ///   Gets a value indicating whether this class map is a subclass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this class map is a subclass; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsSubClass { get; }

        /// <summary>
        ///   Gets the member maps.
        /// </summary>
        /// <value>The member maps.</value>
        public virtual IEnumerable<PersistentMemberMap> MemberMaps
        {
            get { return _memberMaps.AsReadOnly(); }
        }

        /// <summary>
        ///   Gets the sub class maps.
        /// </summary>
        /// <value>The sub class maps.</value>
        public IEnumerable<SubClassMap> SubClassMaps
        {
            get { return _subClassMaps.AsReadOnly(); }
        }

        private void EnsureCreator()
        {
            if (_creator == null)
                _creator = MemberReflectionOptimizer.GetCreator(ClassType);
        }

        /// <summary>
        ///   Creates an instance of the entity.
        /// </summary>
        /// <returns></returns>
        public object CreateInstance()
        {
            if(!_hasProtectedOrPublicConstructor)
                throw new MissingMethodException("No public or protected constructor found on type " + ClassType.FullName);

            if (ClassType.IsAbstract)
                throw new MongoException("Unable to create an instance of an abstract class.");

            EnsureCreator();

            //TODO: figure out how to support custom activators...
            object instance = _creator.Invoke();

            //initialize all default values in case something isn't specified when reader the document.)
            foreach(var memberMap in MemberMaps.Where(x => x.DefaultValue != null))
                memberMap.SetValue(instance, memberMap.DefaultValue);

            return instance;
        }

        /// <summary>
        ///   Gets the class map from discriminator.
        /// </summary>
        /// <param name = "discriminator">The discriminator.</param>
        /// <returns></returns>
        public virtual IClassMap GetClassMapFromDiscriminator(object discriminator)
        {
            return GetClassMapFromDiscriminator(this, discriminator);
        }

        /// <summary>
        ///   Gets the id of the specified entitiy.
        /// </summary>
        /// <param name = "entity">The entity.</param>
        /// <returns></returns>
        public object GetId(object entity)
        {
            if(!HasId)
                throw new InvalidCastException(string.Format("{0} does not have a mapped id.", ClassType));

            return IdMap.GetValue(entity);
        }

        /// <summary>
        ///   Gets the member map from alias.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        /// <returns></returns>
        public PersistentMemberMap GetMemberMapFromAlias(string propertyName)
        {
            if(HasId && IdMap.Alias == propertyName)
                return IdMap;

            return MemberMaps.FirstOrDefault(memberMap => memberMap.Alias == propertyName);
        }

        /// <summary>
        ///   Gets the member map that corresponds to the specified member name.
        /// </summary>
        /// <param name = "memberName">Name of the member.</param>
        /// <returns></returns>
        public PersistentMemberMap GetMemberMapFromMemberName(string memberName)
        {
            if(HasId && IdMap.MemberName == memberName)
                return IdMap;

            return MemberMaps.FirstOrDefault(memberMap => memberMap.MemberName == memberName);
        }

        /// <summary>
        ///   Adds the member map.
        /// </summary>
        /// <param name = "memberMap">The member map.</param>
        internal void AddMemberMap(PersistentMemberMap memberMap)
        {
            _memberMaps.Add(memberMap);
        }

        /// <summary>
        ///   Adds the member maps.
        /// </summary>
        /// <param name = "memberMaps">The member maps.</param>
        internal void AddMemberMaps(IEnumerable<PersistentMemberMap> memberMaps)
        {
            if (memberMaps.Any(m => m.Alias == "_id"))
                throw new MongoException("_id is a reserved MongoDB alias and cannot be used for anything other than an Id column.");

            _memberMaps.AddRange(memberMaps);
        }

        /// <summary>
        ///   Adds the sub class map.
        /// </summary>
        /// <param name = "subClassMap">The sub class map.</param>
        internal void AddSubClassMap(SubClassMap subClassMap)
        {
            _subClassMaps.Add(subClassMap);
            subClassMap.SuperClassMap = this;
        }

        /// <summary>
        ///   Adds the sub class maps.
        /// </summary>
        /// <param name = "subClassMaps">The sub class maps.</param>
        internal void AddSubClassMaps(IEnumerable<SubClassMap> subClassMaps)
        {
            foreach(var subClassMap in subClassMaps)
                AddSubClassMap(subClassMap);
        }

        private static IClassMap GetClassMapFromDiscriminator(IClassMap classMap, object discriminator)
        {
            if(AreObjectsEqual(classMap.Discriminator, discriminator))
                return classMap;

            return
                classMap.SubClassMaps.Select(subClassMap => GetClassMapFromDiscriminator(subClassMap, discriminator)).FirstOrDefault(
                    subSubClassMap => subSubClassMap != null);
        }

        private static bool AreObjectsEqual(object a, object b)
        {
            if(a == null && b == null)
                return true;
            if(a == null || b == null)
                return false;

            if(a is IEnumerable && b is IEnumerable)
            {
                var aEnum = ((IEnumerable)a).GetEnumerator();
                var bEnum = ((IEnumerable)b).GetEnumerator();
                while(aEnum.MoveNext() && bEnum.MoveNext())
                {
                    var v = AreObjectsEqual(aEnum.Current, bEnum.Current);
                    if(!v)
                        return false;
                }
                return true;
            }

            return a.Equals(b);
        }
    }
}