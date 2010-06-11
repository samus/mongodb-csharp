using System;
using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Configuration.Mapping.Auto
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultMemberFinder : IMemberFinder
    {
        ///<summary>
        ///</summary>
        public static readonly DefaultMemberFinder Instance = new DefaultMemberFinder();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMemberFinder"/> class.
        /// </summary>
        private DefaultMemberFinder()
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

                if(getMethod==null || getMethod.IsPrivate || setMethod==null)
                    continue;

                if (setMethod.GetParameters().Length != 1) //an indexer
                    continue;

                yield return prop;
            }

            foreach (var field in type.GetFields()) //all public fields
            {
                if (!field.IsInitOnly && !field.IsLiteral) //readonly
                    yield return field;
            }
        }
    }
}