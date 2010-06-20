using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassOverrides
    {
        private readonly IdOverrides _idOverrides;
        private readonly Dictionary<MemberInfo, MemberOverrides> _memberOverrides;

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
            _idOverrides = null;
            _memberOverrides = new Dictionary<MemberInfo, MemberOverrides>();
        }

        /// <summary>
        /// Gets the id overrides.
        /// </summary>
        /// <returns></returns>
        public IdOverrides GetIdOverrides()
        {
            return _idOverrides;
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
