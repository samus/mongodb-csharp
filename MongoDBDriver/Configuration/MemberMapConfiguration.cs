using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Auto;

namespace MongoDB.Driver.Configuration
{
    public class MemberMapConfiguration
    {
        private readonly MemberOverrides _overrides;

        internal MemberMapConfiguration(MemberOverrides overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            _overrides = overrides;
        }

        public MemberMapConfiguration Alias(string name)
        {
            _overrides.Alias = name;
            return this;
        }

        public MemberMapConfiguration DefaultValue(object defaultValue)
        {
            _overrides.DefaultValue = defaultValue;
            return this;
        }

        public MemberMapConfiguration Ignore()
        {
            _overrides.Ignore = true;
            return this;
        }

        public MemberMapConfiguration PersistNull()
        {
            _overrides.PersistIfNull = true;
            return this;
        }
    }
}