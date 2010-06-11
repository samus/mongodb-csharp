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
        /// Gets the dictionary adapter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IDictionaryAdapter GetDictionaryAdapter(Type type);
    }
}