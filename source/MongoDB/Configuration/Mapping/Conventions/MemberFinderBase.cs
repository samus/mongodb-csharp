using System;
using System.Reflection;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MemberFinderBase
    {
        private readonly BindingFlags _bindingFlags;
        private readonly MemberTypes _memberTypes;
        private readonly Func<MemberInfo, bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFinderBase"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        protected MemberFinderBase(Func<MemberInfo, bool> predicate)
            : this(predicate, MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFinderBase"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="memberTypes">The member types.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        protected MemberFinderBase(Func<MemberInfo, bool> predicate, MemberTypes memberTypes, BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
            _memberTypes = memberTypes;
            _predicate = predicate;
        }

        /// <summary>
        /// Gets the member representing the id if one exists.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected MemberInfo GetMember(Type type)
        {
            var foundMembers = type.FindMembers(_memberTypes, _bindingFlags, IsMatch, null);

            if (foundMembers.Length == 0)
                return null;
            if (foundMembers.Length == 1)
                return foundMembers[0];

            //Todo: use custom exception
            throw new Exception("Too many members found matching the criteria.");
        }

        /// <summary>
        /// Determines whether the specified member info is match.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// 	<c>true</c> if the specified member info is match; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMatch(MemberInfo memberInfo, object criteria)
        {
            return _predicate(memberInfo);
        }
    }
}