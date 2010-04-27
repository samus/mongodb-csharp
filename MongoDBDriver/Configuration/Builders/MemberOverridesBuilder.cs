using System;
using MongoDB.Driver.Configuration.Mapping.Auto;

namespace MongoDB.Driver.Configuration.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class MemberOverridesBuilder
    {
        private readonly MemberOverrides _overrides;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberMapConfiguration"/> class.
        /// </summary>
        /// <param name="overrides">The overrides.</param>
        internal MemberOverridesBuilder(MemberOverrides overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            _overrides = overrides;
        }

        /// <summary>
        /// Aliases the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public MemberOverridesBuilder Alias(string name)
        {
            _overrides.Alias = name;
            return this;
        }

        /// <summary>
        /// Defaults the value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public MemberOverridesBuilder DefaultValue(object defaultValue)
        {
            _overrides.DefaultValue = defaultValue;
            return this;
        }

        /// <summary>
        /// Ignores this instance.
        /// </summary>
        /// <returns></returns>
        public MemberOverridesBuilder Ignore()
        {
            _overrides.Ignore = true;
            return this;
        }

        /// <summary>
        /// Persists the null.
        /// </summary>
        /// <returns></returns>
        public MemberOverridesBuilder PersistNull()
        {
            _overrides.PersistIfNull = true;
            return this;
        }
    }
}