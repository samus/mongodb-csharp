using System;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIdConvention
    {
        /// <summary>
        /// Gets the member representing the id if one exists.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        MemberInfo GetIdMember(Type classType);
    }
}