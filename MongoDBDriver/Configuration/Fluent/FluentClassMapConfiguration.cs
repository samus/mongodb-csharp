using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Auto;
using System.Linq.Expressions;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Fluent
{
    public class FluentClassMapConfiguration<T>
    {
        private readonly ClassOverrides _overrides;

        public FluentClassMapConfiguration(ClassOverrides overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            _overrides = overrides;
        }

        public void CollectionNameIs(string name)
        {
            _overrides.CollectionName = name;
        }

        public FluentMemberMapConfiguration Member(MemberInfo member)
        {
            var overrides = _overrides.GetOverridesFor(member);
            return new FluentMemberMapConfiguration(overrides);
        }

        public FluentMemberMapConfiguration Member(string name)
        {
            var members = typeof(T).GetMember(name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (members == null || members.Length == 0)
                throw new InvalidOperationException("No member was found.");
            else if (members.Length > 1)
                throw new InvalidOperationException("More than one member matched the specified name.");

            return Member(members[0]);
        }

        public FluentMemberMapConfiguration Member(Expression<Func<T, object>> member)
        {
            throw new NotImplementedException();
        }
    }
}