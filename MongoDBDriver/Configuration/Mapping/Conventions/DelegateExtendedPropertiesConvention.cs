using System;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateExtendedPropertiesConvention : MemberFinderBase, IExtendedPropertiesConvention
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateExtendedPropertiesConvention"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public DelegateExtendedPropertiesConvention(Func<MemberInfo, bool> predicate)
            : base(predicate)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateExtendedPropertiesConvention"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="memberTypes">The member types.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        public DelegateExtendedPropertiesConvention(Func<MemberInfo, bool> predicate, MemberTypes memberTypes, BindingFlags bindingFlags)
            : base(predicate, memberTypes, bindingFlags)
        { }

        /// <summary>
        /// Gets the member representing extended properties if one exists.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public MemberInfo GetExtendedPropertiesMember(Type classType)
        {
            return GetMember(classType);
        }
    }
}