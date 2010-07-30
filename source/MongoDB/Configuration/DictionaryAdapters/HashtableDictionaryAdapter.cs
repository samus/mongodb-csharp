using System;
using System.Collections;
using MongoDB.Configuration.Mapping.Util;

namespace MongoDB.Configuration.DictionaryAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class HashtableDictionaryAdapter : IDictionaryAdapter
    {
        /// <summary>
        /// Gets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        public Type KeyType
        {
            get { return typeof(object); }
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
            if(document == null)
                return null;

            var hashtable = new Hashtable();

            foreach (var pair in document)
                hashtable.Add(pair.Key, pair.Value);

            return hashtable;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public Document GetDocument(object collection)
        {
            var hashtable = collection as Hashtable;
            if (hashtable == null)
                return new Document();

            var doc = new Document();
            foreach (DictionaryEntry entry in hashtable)
                doc.Add(ValueConverter.ConvertKey(entry.Key), entry.Value);

            return doc;
        }
    }
}