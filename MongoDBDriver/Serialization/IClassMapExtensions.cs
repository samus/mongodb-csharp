using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization
{
    internal static class IClassMapExtensions
    {

        public static bool ShouldPersistDiscriminator(this IClassMap classMap)
        {
            return (classMap.IsPolymorphic && classMap.HasDiscriminator) || classMap.IsSubClass;
        }

    }
}
