using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    public interface IDefaultValueConvention
    {
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object GetDefaultValue(Type type);
    }
}