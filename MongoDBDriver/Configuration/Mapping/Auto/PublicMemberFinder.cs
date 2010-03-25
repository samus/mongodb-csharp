using System;
using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Auto
{
    public class PublicMemberFinder : IMemberFinder
    {
        public static readonly PublicMemberFinder Instance = new PublicMemberFinder();

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicMemberFinder"/> class.
        /// </summary>
        private PublicMemberFinder()
        { }

        /// <summary>
        /// Finds the members.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<MemberInfo> FindMembers(Type type)
        {
            foreach (var prop in type.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
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