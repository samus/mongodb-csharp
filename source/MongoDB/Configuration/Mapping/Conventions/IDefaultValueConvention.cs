using System;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
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