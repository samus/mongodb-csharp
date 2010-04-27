using System;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentMemberMap : PersistentMemberMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMemberMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="persistNull">if set to <c>true</c> [persist null].</param>
        public DocumentMemberMap(string memberName, Func<object, object> getter, Action<object, object> setter, string alias, bool persistNull)
            : base(memberName, typeof(Document), getter, setter, null, alias, persistNull)
        { }
    }
}