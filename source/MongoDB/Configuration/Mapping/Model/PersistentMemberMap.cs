using System;

namespace MongoDB.Driver.Configuration.Mapping.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PersistentMemberMap : MemberMapBase
    {
        private readonly string _alias;
        private readonly object _defaultValue;
        private readonly bool _persistNull;

        /// <summary>
        /// Gets the alias in which to store the value.
        /// </summary>
        /// <value>The name.</value>
        public string Alias
        {
            get { return _alias; }
        }

        /// <summary>
        /// Gets a value indicating whether [default value].
        /// </summary>
        /// <value><c>true</c> if [default value]; otherwise, <c>false</c>.</value>
        public object DefaultValue
        {
            get { return _defaultValue; }
        }

        /// <summary>
        /// Gets a value indicating whether or not null should be persisted to the database.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the null should be persisted; otherwise, <c>false</c>.
        /// </value>
        public bool PersistNull
        {
            get { return _persistNull; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentMemberMap"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberReturnType">Type of the member return.</param>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="persistNull">if set to <c>true</c> [persist null].</param>
        public PersistentMemberMap(string memberName, Type memberReturnType, Func<object, object> getter, Action<object, object> setter, object defaultValue, string alias, bool persistNull)
            : base(memberName, memberReturnType, getter, setter)
        {
            _alias = alias;
            _defaultValue = defaultValue;
            _persistNull = persistNull;
        }
    }
}