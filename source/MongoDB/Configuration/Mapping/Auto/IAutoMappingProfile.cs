using System;
using System.Collections.Generic;
using System.Reflection;

using MongoDB.Configuration.CollectionAdapters;
using MongoDB.Configuration.IdGenerators;
using MongoDB.Configuration.DictionaryAdapters;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAutoMappingProfile
    {
        /// <summary>
        /// Finds the extended properties member.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns></returns>
        MemberInfo FindExtendedPropertiesMember(Type classType);

        /// <summary>
        /// Gets the id member for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        MemberInfo FindIdMember(Type classType);

        /// <summary>
        /// Finds the members to map for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        IEnumerable<MemberInfo> FindMembers(Type classType);

        /// <summary>
        /// Gets the alias for the specified member.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        string GetAlias(Type classType, MemberInfo member);

        /// <summary>
        /// Gets the collection name for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        string GetCollectionName(Type classType);

        /// <summary>
        /// Gets the collection adapter.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        ICollectionAdapter GetCollectionAdapter(Type classType, MemberInfo member, Type memberReturnType);

        /// <summary>
        /// Gets the type of the collection element.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        Type GetCollectionElementType(Type classType, MemberInfo member, Type memberReturnType);

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        object GetDefaultValue(Type classType, MemberInfo member);

        /// <summary>
        /// Gets the dictionary adadpter.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <returns></returns>
        IDictionaryAdapter GetDictionaryAdapter(Type classType, MemberInfo member, Type memberReturnType);

        /// <summary>
        /// Gets the descriminator for the class type.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        object GetDiscriminator(Type classType);

        /// <summary>
        /// Gets the discriminator alias.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns></returns>
        string GetDiscriminatorAlias(Type classType);

        /// <summary>
        /// Gets the id generator for the member.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        IIdGenerator GetIdGenerator(Type classType, MemberInfo member);

        /// <summary>
        /// Gets the unsaved value for the id.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        object GetIdUnsavedValue(Type classType, MemberInfo member);

        /// <summary>
        /// Gets a value indicating whether the member should be persisted if it is it's default value.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        bool GetPersistDefaultValue(Type classType, MemberInfo member);

        /// <summary>
        /// Indicates whether the class type is a sub class.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns>
        /// 	<c>true</c> if the classType is a sub class; otherwise, <c>false</c>.
        /// </returns>
        bool IsSubClass(Type classType);
    }
}