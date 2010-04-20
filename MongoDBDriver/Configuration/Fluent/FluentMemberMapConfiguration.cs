using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Auto;

namespace MongoDB.Driver.Configuration.Fluent
{
    public class FluentMemberMapConfiguration
    {
        private readonly MemberOverrides _overrides;

        internal FluentMemberMapConfiguration(MemberOverrides overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            _overrides = overrides;
        }

        public FluentMemberMapConfiguration Alias(string name)
        {
            _overrides.Alias = name;
            return this;
        }

        public FluentMemberMapConfiguration DefaultValueIs(object defaultValue)
        {
            _overrides.DefaultValue = defaultValue;
            return this;
        }

        public FluentMemberMapConfiguration Ignore()
        {
            _overrides.Ignore = true;
            return this;
        }

        public FluentMemberMapConfiguration PersistNull()
        {
            _overrides.PersistIfNull = true;
            return this;
        }
    }
}