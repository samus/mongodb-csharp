using System;

namespace MongoDB.Driver.Serialization
{
    /// <summary>
    /// Provides a way to get a name for the _type database field and back.
    /// </summary>
    public interface ITypeNameProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object GetName(Type type);

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        Type GetType(object typeName);
    }
}