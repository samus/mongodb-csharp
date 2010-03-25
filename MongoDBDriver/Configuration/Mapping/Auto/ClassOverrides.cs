using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    public class ClassOverrides
    {
        readonly Dictionary<MemberInfo, MemberOverrides> _memberOverrides;

        public string CollectionName { get; set; }

        public ClassOverrides()
        {
            _memberOverrides = new Dictionary<MemberInfo, MemberOverrides>();
        }

        public MemberOverrides GetOverridesFor(MemberInfo memberInfo)
        {
            MemberOverrides memberOverrides;
            if (!_memberOverrides.TryGetValue(memberInfo, out memberOverrides))
                memberOverrides = _memberOverrides[memberInfo] = new MemberOverrides();

            return memberOverrides;
        }
    }
}
