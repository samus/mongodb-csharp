using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Configuration.Mapping.Model
{
    public class ClassMap : ClassMapBase
    {
        private string _collectionName;
        private object _discriminator;
        private string _discriminatorPropertyName;
        private ExtendedPropertiesMap _extendedPropertiesMap;
        private IdMap _idMap;
        private readonly List<SubClassMap> _subClassMaps;

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public override string CollectionName
        {
            get { return _collectionName; }
            internal set { _collectionName = value; }
        }

        /// <summary>
        /// Gets the alias used to store the discriminator.
        /// </summary>
        /// <value>The discriminator alias.</value>
        public override string DiscriminatorAlias
        {
            get { return _discriminatorPropertyName; }
            internal set { _discriminatorPropertyName = value; }
        }

        /// <summary>
        /// Gets the extended properties map.
        /// </summary>
        /// <value>The extended properties map.</value>
        public override ExtendedPropertiesMap ExtendedPropertiesMap
        {
            get { return _extendedPropertiesMap; }
            internal set { _extendedPropertiesMap = value; }
        }

        /// <summary>
        /// Gets the id map.
        /// </summary>
        /// <value>The id map.</value>
        public override IdMap IdMap
        {
            get { return _idMap; }
            internal set { _idMap = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this class map is polymorphic.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class map is polymorphic; otherwise, <c>false</c>.
        /// </value>
        public override bool IsPolymorphic
        {
            get { return _subClassMaps.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this class map is a subclass.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class map is a subclass; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSubClass
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMap"/> class.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        public ClassMap(Type classType)
            : base(classType)
        {
            _subClassMaps = new List<SubClassMap>();
        }

        /// <summary>
        /// Gets the class map from discriminator.
        /// </summary>
        /// <param name="discriminator">The discriminator.</param>
        /// <returns></returns>
        public override IClassMap GetClassMapFromDiscriminator(object discriminator)
        {
            if (Discriminator == null)
            {
                if (discriminator == null)
                    return this;
            }
            else if (Discriminator.Equals(discriminator))
                return this;

            foreach (var subClassMap in _subClassMaps)
                if (subClassMap.Discriminator.Equals(discriminator))
                    return subClassMap;

            return null;
        }

        /// <summary>
        /// Adds the sub class map.
        /// </summary>
        /// <param name="subClassMap">The sub class map.</param>
        internal void AddSubClassMap(SubClassMap subClassMap)
        {
            _subClassMaps.Add(subClassMap);
            subClassMap.SuperClassMap = this;
        }

        /// <summary>
        /// Adds the sub class maps.
        /// </summary>
        /// <param name="subClassMaps">The sub class maps.</param>
        internal void AddSubClassMaps(IEnumerable<SubClassMap> subClassMaps)
        {
            foreach (var subClassMap in subClassMaps)
                AddSubClassMap(subClassMap);
        }
    }
}