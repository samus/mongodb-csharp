using System;
using System.Reflection;

namespace MongoDB.Driver.Util
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member">The member.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns></returns>
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

        /// <summary>
        /// Determines whether [is open type assignable from] [the specified open type].
        /// </summary>
        /// <param name="openType">Type of the open.</param>
        /// <param name="closedType">Type of the closed.</param>
        /// <returns>
        /// 	<c>true</c> if [is open type assignable from] [the specified open type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOpenTypeAssignableFrom(this Type openType, Type closedType)
        {
            if (!openType.IsGenericTypeDefinition)
                throw new ArgumentException("Must be an open generic type.", "openType");
            if (!closedType.IsGenericType || closedType.IsGenericTypeDefinition)
                return false;

            var openArgs = openType.GetGenericArguments();
            var closedArgs = closedType.GetGenericArguments();
            if (openArgs.Length != closedArgs.Length)
                return false;
            try
            {
                var newType = openType.MakeGenericType(closedArgs);
                return newType.IsAssignableFrom(closedType);
            }
            catch
            {
                //we don't really care here, it just means the answer is false.
                return false;
            }
        }
    }
}
