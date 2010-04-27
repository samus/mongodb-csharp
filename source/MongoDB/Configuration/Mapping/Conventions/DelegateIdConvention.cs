using System;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateIdConvention : MemberFinderBase, IIdConvention
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateIdConvention"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public DelegateIdConvention(Func<MemberInfo, bool> predicate)
            : base(predicate)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateIdConvention"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="memberTypes">The member types.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        public DelegateIdConvention(Func<MemberInfo, bool> predicate, MemberTypes memberTypes, BindingFlags bindingFlags)
            : base(predicate, memberTypes, bindingFlags)
        { }

        /// <summary>
        /// Gets the member representing the id if one exists.
        /// </summary>
        /// <param name="classType">Type of the entity.</param>
        /// <returns></returns>
        public MemberInfo GetIdMember(Type classType)
        {
            return GetMember(classType);
        }
    }
}