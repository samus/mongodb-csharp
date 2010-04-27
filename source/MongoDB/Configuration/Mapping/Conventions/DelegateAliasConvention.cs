using System;
using System.Reflection;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateAliasConvention : IAliasConvention
    {
        readonly Func<MemberInfo, string> _alias;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateAliasConvention"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        public DelegateAliasConvention(Func<MemberInfo, string> alias)
        {
            _alias = alias;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public string GetAlias(MemberInfo memberInfo)
        {
            return _alias(memberInfo);
        }
    }
}