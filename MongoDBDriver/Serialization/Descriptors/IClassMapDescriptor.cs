using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Configuration.Mapping.Model;

namespace MongoDB.Driver.Serialization.Descriptors
{
    internal interface IClassMapDescriptor : IPropertyDescriptor
    {
        /// <summary>
        /// Gets the member map.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        PersistentMemberMap GetMemberMap(string name);
    }
}