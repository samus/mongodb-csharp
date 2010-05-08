using System;
using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class PublicOrProtectedMemberFinder : IMemberFinder
    {
        ///<summary>
        ///</summary>
        public static readonly PublicOrProtectedMemberFinder Instance = new PublicOrProtectedMemberFinder();

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicOrProtectedMemberFinder"/> class.
        /// </summary>
        private PublicOrProtectedMemberFinder()
        {
        }

        /// <summary>
        /// Finds the members.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<MemberInfo> FindMembers(Type type)
        {
            foreach (var prop in type.GetProperties(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public))
            {
                var getMethod = prop.GetGetMethod(true);
                var setMethod = prop.GetSetMethod(true);

                if(getMethod==null||setMethod==null)
                    continue;

                if(getMethod.IsPrivate||setMethod.IsPrivate)
                    continue;

                yield return prop;
            }

            foreach (var field in type.GetFields())
            {
                if (!field.IsInitOnly && !field.IsLiteral)
                    yield return field;
            }
        }
    }
}