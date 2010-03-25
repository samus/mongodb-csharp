using System;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    public interface IExtendedPropertiesConvention
    {
        /// <summary>
        /// Gets the member representing extended properties if one exists.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        MemberInfo GetExtendedPropertiesMember(Type classType);
    }
}