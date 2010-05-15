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
        /// <param name="document">The document.</param>
        /// <returns></returns>
        object CreateDictionary(Type valueType, Document document);

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns></returns>
        Document GetDocument(object dictionary, Type valueType);
    }
}