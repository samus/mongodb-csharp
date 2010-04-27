using System;
using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMemberFinder
    {
        /// <summary>
        /// Finds the members.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEnumerable<MemberInfo> FindMembers(Type type);
    }
}