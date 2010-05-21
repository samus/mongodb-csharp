using System;

namespace MongoDB.Configuration.DictionaryAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentDictionaryAdapter : IDictionaryAdapter
    {
        /// <summary>
        /// Gets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        public Type KeyType
        {
            get { return typeof(string); }
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public Type ValueType
        {
            get { return typeof(object); }
        }

        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public object CreateDictionary(Document document)
        {
            return document;
        }

        /// <summary>
        /// Gets the pairs.
        /// </summary>
        /// <param name="dictionary">The collection.</param>
        /// <returns></returns>
        public Document GetDocument(object dictionary)
        {
            return dictionary as Document;
        }
    }
}