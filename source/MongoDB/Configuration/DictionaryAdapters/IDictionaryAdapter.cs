using System;

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
        /// <returns></returns>
        Document GetDocument(object dictionary);
    }
}