using System;
using MongoDB.Configuration.Mapping.Auto;
using System.Linq.Expressions;
using System.Reflection;

namespace MongoDB.Configuration.Builders
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassOverridesBuilder<T>
    {
        private readonly ClassOverrides _overrides;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassOverridesBuilder&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="overrides">The overrides.</param>
        internal ClassOverridesBuilder(ClassOverrides overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            _overrides = overrides;
        }

        /// <summary>
        /// Collections the name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void CollectionName(string name)
        {
            _overrides.CollectionName = name;
        }

        /// <summary>
        /// Ids the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public IdOverridesBuilder Id(MemberInfo member)
        {
            var overrides = new IdOverrides { Member = member };
            return new IdOverridesBuilder(overrides);
        }

        /// <summary>
        /// Ids the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IdOverridesBuilder Id(string name)
        {
            var members = typeof(T).GetMember(name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (members == null || members.Length == 0)
                throw new InvalidOperationException("No member was found.");
            if (members.Length > 1)
                throw new InvalidOperationException("More than one member matched the specified name.");

            return Id(members[0]);
        }

        /// <summary>
        /// Ids the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public IdOverridesBuilder Id(Expression<Func<T, object>> member)
        {
            var mex = GetMemberExpression(member);
            return Id(mex.Member.Name);
        }

        /// <summary>
        /// Members the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public MemberOverridesBuilder Member(MemberInfo member)
        {
            var overrides = _overrides.GetOverridesFor(member);
            return new MemberOverridesBuilder(overrides);
        }

        /// <summary>
        /// Members the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public MemberOverridesBuilder Member(string name)
        {
            var members = typeof(T).GetMember(name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (members == null || members.Length == 0)
                throw new InvalidOperationException("No member was found.");
            if (members.Length > 1)
                throw new InvalidOperationException("More than one member matched the specified name.");

            return Member(members[0]);
        }

        /// <summary>
        /// Members the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public MemberOverridesBuilder Member(Expression<Func<T, object>> member)
        {
            var mex = GetMemberExpression(member);
            return Member(mex.Member.Name);
        }

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        private MemberExpression GetMemberExpression(Expression<Func<T, object>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = member.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            return memberExpression;
        }
    }
}