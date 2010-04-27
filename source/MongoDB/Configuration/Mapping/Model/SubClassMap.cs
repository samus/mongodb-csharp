using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class SubClassMap : ClassMapBase
    {
        private IClassMap _superClassMap;

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public override string CollectionName
        {
            get { return _superClassMap.CollectionName; }
            internal set { throw new NotSupportedException("Cannot set the collection name on a subclass map."); }
        }

        /// <summary>
        /// Gets the alias used to store the discriminator.
        /// </summary>
        /// <value>The discriminator alias.</value>
        public override string DiscriminatorAlias
        {
            get { return _superClassMap.DiscriminatorAlias; }
            internal set { throw new NotSupportedException("Cannot set the discriminator property name on a subclass map."); }
        }

        /// <summary>
        /// Gets the extended properties map.
        /// </summary>
        /// <value>The extended properties map.</value>
        public override ExtendedPropertiesMap ExtendedPropertiesMap
        {
            get { return _superClassMap.ExtendedPropertiesMap; }
            internal set { throw new NotSupportedException("Cannot set the extended properties map on a subclass map."); }
        }

        /// <summary>
        /// Gets the id map.
        /// </summary>
        /// <value>The id map.</value>
        public override IdMap IdMap
        {
            get { return _superClassMap.IdMap; }
            internal set { throw new NotSupportedException("Cannot set the id map on a subclass map."); }
        }

        /// <summary>
        /// Gets a value indicating whether this class map is a subclass.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class map is a subclass; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSubClass
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the member maps.
        /// </summary>
        /// <value>The member maps.</value>
        public override IEnumerable<PersistentMemberMap> MemberMaps
        {
            get{return _superClassMap.MemberMaps.Concat(base.MemberMaps);}
        }

        /// <summary>
        /// Gets or sets the super class map.
        /// </summary>
        /// <value>The super class map.</value>
        public IClassMap SuperClassMap
        {
            get { return _superClassMap; }
            internal set { _superClassMap = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubClassMap"/> class.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        public SubClassMap(Type classType)
            : base(classType)
        { }
    }
}