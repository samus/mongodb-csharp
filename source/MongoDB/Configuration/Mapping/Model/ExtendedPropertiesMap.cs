using System;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtendedPropertiesMap : MemberMapBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedPropertiesMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        public ExtendedPropertiesMap(string memberName, Type memberReturnType, Func<object, object> getter, Action<object, object> setter)
            : base(memberName, memberReturnType, getter, setter)
        { }
    }
}