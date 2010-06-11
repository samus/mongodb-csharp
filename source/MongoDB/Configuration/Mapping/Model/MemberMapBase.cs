using System;
using MongoDB.Configuration.Mapping.Util;

namespace MongoDB.Configuration.Mapping.Model
{
    /// <summary>
    /// </summary>
    public class MemberMapBase
    {
        private readonly Func<object, object> _getter;
        private readonly Action<object, object> _setter;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MemberMapBase" /> class.
        /// </summary>
        /// <param name = "memberName">Name of the member.</param>
        /// <param name = "memberReturnType">Type of the member return.</param>
        /// <param name = "getter">The getter.</param>
        /// <param name = "setter">The setter.</param>
        protected MemberMapBase(string memberName, Type memberReturnType, Func<object, object> getter, Action<object, object> setter)
        {
            if(memberReturnType == null)
                throw new ArgumentNullException("memberReturnType");

            _getter = getter;
            MemberName = memberName;
            MemberReturnType = memberReturnType;
            _setter = setter;
        }

        /// <summary>
        ///   Gets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        public string MemberName { get; private set; }

        /// <summary>
        ///   Gets the type of the member return.
        /// </summary>
        /// <value>The type of the member return.</value>
        public Type MemberReturnType { get; private set; }

        /// <summary>
        ///   Gets the value.
        /// </summary>
        /// <param name = "instance">The instance.</param>
        /// <returns></returns>
        public virtual object GetValue(object instance)
        {
            return _getter(instance);
        }

        /// <summary>
        ///   Sets the value on the specified instance.
        /// </summary>
        /// <param name = "instance">The instance.</param>
        /// <param name = "value">The value.</param>
        public virtual void SetValue(object instance, object value)
        {
            try
            {
                value = ValueConverter.Convert(value, MemberReturnType);
            }
            catch(MongoException exception)
            {
                throw new MongoException("Con not convert value on type " + instance.GetType(),exception);
            }

            _setter(instance, value);
        }
    }
}