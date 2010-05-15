using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Configuration.DictionaryAdapters
{
    public class HashtableDictionaryAdapter : IDictionaryAdapter
    {
        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public object CreateDictionary(Type valueType, Document document)
        {
            var hashtable = new Hashtable();

            foreach (var pair in document)
                hashtable.Add((string)pair.Key, pair.Value);

            return hashtable;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns></returns>
        public Document GetDocument(object collection, Type valueType)
        {
            var hashtable = collection as Hashtable;
            if (hashtable == null)
                return new Document();

            var doc = new Document();
            foreach (DictionaryEntry entry in hashtable)
                doc.Add(entry.Key.ToString(), entry.Value);

            return doc;
        }
    }
}