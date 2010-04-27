using System;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIdUnsavedValueConvention
    {
        /// <summary>
        /// Gets the unsaved value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object GetUnsavedValue(Type type);
    }
}