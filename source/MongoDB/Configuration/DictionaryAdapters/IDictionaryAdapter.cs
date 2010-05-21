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
        /// Gets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        Type KeyType { get; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        Type ValueType { get; }

        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        object CreateDictionary(Document document);

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns></returns>
        Document GetDocument(object dictionary);
    }
}