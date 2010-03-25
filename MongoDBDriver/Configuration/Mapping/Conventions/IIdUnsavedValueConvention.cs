using System;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
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