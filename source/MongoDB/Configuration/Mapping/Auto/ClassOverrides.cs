using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassOverrides
    {
        readonly Dictionary<MemberInfo, MemberOverrides> _memberOverrides;

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public string CollectionName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassOverrides"/> class.
        /// </summary>
        public ClassOverrides()
        {
            _memberOverrides = new Dictionary<MemberInfo, MemberOverrides>();
        }

        /// <summary>
        /// Gets the overrides for.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public MemberOverrides GetOverridesFor(MemberInfo memberInfo)
        {
            MemberOverrides memberOverrides;
            if (!_memberOverrides.TryGetValue(memberInfo, out memberOverrides))
                memberOverrides = _memberOverrides[memberInfo] = new MemberOverrides();

            return memberOverrides;
        }
    }
}
