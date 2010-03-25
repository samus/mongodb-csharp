using System;
using System.Collections.Generic;

namespace MongoDB.Driver.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClassMap
    {
        /// <summary>
        /// Gets the type of class to which this map pertains.
        /// </summary>
        /// <value>The type of the class.</value>
        Type ClassType { get; }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        string CollectionName { get; }

        /// <summary>
        /// Gets the discriminator.
        /// </summary>
        /// <value>The discriminator.</value>
        object Discriminator { get; }

        /// <summary>
        /// Gets the alias used to store the discriminator.
        /// </summary>
        /// <value>The discriminator alias.</value>
        string DiscriminatorAlias { get; }

        /// <summary>
        /// Gets the extended properties map.
        /// </summary>
        /// <value>The extended properties map.</value>
        ExtendedPropertiesMap ExtendedPropertiesMap { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has discriminator.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has discriminator; otherwise, <c>false</c>.
        /// </value>
        bool HasDiscriminator { get; }

        /// <summary>
        /// Gets a value indicating whether the class map has extended properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the class map has extended properties; otherwise, <c>false</c>.
        /// </value>
        bool HasExtendedProperties { get; }

        /// <summary>
        /// Gets a value indicating whether the class map has an id.
        /// </summary>
        /// <value><c>true</c> if the class map has an id; otherwise, <c>false</c>.</value>
        bool HasId { get; }

        /// <summary>
        /// Gets the id map.
        /// </summary>
        /// <value>The id map.</value>
        IdMap IdMap { get; }

        /// <summary>
        /// Gets a value indicating whether this class map is polymorphic.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class map is polymorphic; otherwise, <c>false</c>.
        /// </value>
        bool IsPolymorphic { get; }

        /// <summary>
        /// Gets a value indicating whether this class map is a subclass.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class map is a subclass; otherwise, <c>false</c>.
        /// </value>
        bool IsSubClass { get; }

        /// <summary>
        /// Gets the member maps.
        /// </summary>
        /// <value>The member maps.</value>
        IEnumerable<PersistentMemberMap> MemberMaps { get; }

        /// <summary>
        /// Creates an instance of the entity.
        /// </summary>
        /// <returns></returns>
        object CreateInstance();

        /// <summary>
        /// Gets the class map from the specified discriminator.
        /// </summary>
        /// <param name="discriminator">The discriminator.</param>
        /// <returns></returns>
        IClassMap GetClassMapFromDiscriminator(object discriminator);

        /// <summary>
        /// Gets the id of the specified entitiy.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        object GetId(object entity);

        /// <summary>
        /// Gets the member map that corresponds to the specified alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        PersistentMemberMap GetMemberMapFromAlias(string alias);

        /// <summary>
        /// Gets the member map that corresponds to the specified member name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        PersistentMemberMap GetMemberMapFromMemberName(string memberName);
    }
}