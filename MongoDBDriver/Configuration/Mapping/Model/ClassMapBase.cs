using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Configuration.Mapping.Model
{
    public abstract class ClassMapBase : IClassMap
    {
        private Type _classType;
        private object _discriminator;
        private readonly List<PersistentMemberMap> _memberMaps;

        /// <summary>
        /// Gets the type of class to which this map pertains.
        /// </summary>
        /// <value>The type of the class.</value>
        public Type ClassType
        {
            get { return _classType; }
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public abstract string CollectionName { get; internal set; }

        /// <summary>
        /// Gets the discriminator.
        /// </summary>
        /// <value>The discriminator.</value>
        public object Discriminator
        {
            get { return _discriminator; }
            internal set { _discriminator = value; }
        }

        /// <summary>
        /// Gets the alias used to store the discriminator.
        /// </summary>
        /// <value>The discriminator alias.</value>
        public abstract string DiscriminatorAlias { get; internal set; }

        /// <summary>
        /// Gets the extended properties map.
        /// </summary>
        /// <value>The extended properties map.</value>
        public abstract ExtendedPropertiesMap ExtendedPropertiesMap { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance has discriminator.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has discriminator; otherwise, <c>false</c>.
        /// </value>
        public bool HasDiscriminator
        {
            get { return this.Discriminator != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the class map has extended properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the class map has extended properties; otherwise, <c>false</c>.
        /// </value>
        public bool HasExtendedProperties
        {
            get { return this.ExtendedPropertiesMap != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the class map has an id.
        /// </summary>
        /// <value><c>true</c> if the class map has an id; otherwise, <c>false</c>.</value>
        public virtual bool HasId
        {
            get { return IdMap != null; }
        }

        /// <summary>
        /// Gets the id map.
        /// </summary>
        /// <value>The id map.</value>
        public abstract IdMap IdMap { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this class map is polymorphic.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class map is polymorphic; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsPolymorphic { get; }

        /// <summary>
        /// Gets a value indicating whether this class map is a subclass.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class map is a subclass; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsSubClass { get; }

        /// <summary>
        /// Gets the member maps.
        /// </summary>
        /// <value>The member maps.</value>
        public virtual IEnumerable<PersistentMemberMap> MemberMaps
        {
            get { return new ReadOnlyCollection<PersistentMemberMap>(_memberMaps); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMapBase"/> class.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        public ClassMapBase(Type classType)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");

            _classType = classType;
            _memberMaps = new List<PersistentMemberMap>();
        }

        /// <summary>
        /// Creates an instance of the entity.
        /// </summary>
        /// <returns></returns>
        public object CreateInstance()
        {
            //TODO: figure out how to support custom activators...
            var instance = Activator.CreateInstance(ClassType);

            //initialize all default values in case something isn't specified when reader the document.
            foreach (var memberMap in MemberMaps.Where(x => x.DefaultValue != null))
                memberMap.SetValue(instance, memberMap.DefaultValue);

            return instance;
        }

        /// <summary>
        /// Gets the class map from discriminator.
        /// </summary>
        /// <param name="discriminator">The discriminator.</param>
        /// <returns></returns>
        public abstract IClassMap GetClassMapFromDiscriminator(object discriminator);

        /// <summary>
        /// Gets the id of the specified entitiy.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public object GetId(object entity)
        {
            if (!HasId)
                throw new InvalidCastException(string.Format("{0} does not have a mapped id.", _classType));

            return IdMap.GetValue(entity);
        }

        /// <summary>
        /// Gets the member map from alias.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public PersistentMemberMap GetMemberMapFromAlias(string propertyName)
        {
            if (HasId && IdMap.Alias == propertyName)
                return IdMap;

            foreach (var memberMap in MemberMaps)
            {
                if (memberMap.Alias == propertyName)
                    return memberMap;
            }

            return null;
        }

        /// <summary>
        /// Gets the member map that corresponds to the specified member name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        public PersistentMemberMap GetMemberMapFromMemberName(string memberName)
        {
            if (HasId && IdMap.MemberName == memberName)
                return IdMap;

            foreach (var memberMap in MemberMaps)
            {
                if (memberMap.MemberName == memberName)
                    return memberMap;
            }

            return null;
        }

        /// <summary>
        /// Adds the member map.
        /// </summary>
        /// <param name="memberMap">The member map.</param>
        internal void AddMemberMap(PersistentMemberMap memberMap)
        {
            _memberMaps.Add(memberMap);
        }

        /// <summary>
        /// Adds the member maps.
        /// </summary>
        /// <param name="memberMaps">The member maps.</param>
        internal void AddMemberMaps(IEnumerable<PersistentMemberMap> memberMaps)
        {
            _memberMaps.AddRange(memberMaps);
        }
    }
}