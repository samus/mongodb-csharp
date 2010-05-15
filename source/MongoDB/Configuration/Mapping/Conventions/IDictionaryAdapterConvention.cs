using System;

using MongoDB.Configuration.DictionaryAdapters;

namespace MongoDB.Configuration.Mapping.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDictionarynAdapterConvention
    {
        /// <summary>
        /// Gets the type of the dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IDictionaryAdapter GetDictionaryType(Type type);

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        Type GetValueType(Type type);
    }
}