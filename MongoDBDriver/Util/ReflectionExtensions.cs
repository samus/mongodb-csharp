using System;
using System.Reflection;

namespace MongoDB.Driver.Util
{
    internal static class ReflectionExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            var atts = member.GetCustomAttributes(typeof(T), inherit);
            if (atts.Length > 0)
                return (T)atts[0];

            return null;
        }

        /// <summary>
        /// Gets the return type of the member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static Type GetReturnType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
            }

            throw new NotSupportedException("Only fields, properties, and methods are supported.");
        }
    }
}
