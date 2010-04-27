using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAliasConvention
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        string GetAlias(MemberInfo member);
    }
}