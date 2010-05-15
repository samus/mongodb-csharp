using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Configuration.DictionaryAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDictionaryAdapter
    {
        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="pairs">The pairs.</param>
        /// <returns></returns>
        object CreateDictionary(Type valueType, DictionaryEntry[] pairs);

        /// <summary>
        /// Gets the pairs.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        IEnumerable<DictionaryEntry> GetPairs(object collection);
    }
}