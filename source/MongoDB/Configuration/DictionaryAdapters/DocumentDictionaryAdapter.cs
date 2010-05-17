using System;

namespace MongoDB.Configuration.DictionaryAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentDictionaryAdapter : IDictionaryAdapter
    {
        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public object CreateDictionary(Type valueType, Document document)
        {
            return document;
        }

        /// <summary>
        /// Gets the pairs.
        /// </summary>
        /// <param name="dictionary">The collection.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns></returns>
        public Document GetDocument(object dictionary, Type valueType)
        {
            return dictionary as Document;
        }
    }
}