using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Configuration.Mapping.Util;

namespace MongoDB.Configuration.DictionaryAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericDictionaryDictionaryAdapter<TKey, TValue> : IDictionaryAdapter
    {
        /// <summary>
        /// Gets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        public Type KeyType 
        { 
            get { return typeof(TKey); } 
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public Type ValueType
        {
            get { return typeof(TValue); }
        }

        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public object CreateDictionary(Document document)
        {
            return document.ToDictionary(pair => (TKey)Convert.ChangeType(pair.Key, typeof(TKey)), pair => (TValue)pair.Value);
        }

        /// <summary>
        /// Gets the pairs.
        /// </summary>
        /// <param name="dictionary">The collection.</param>
        /// <returns></returns>
        public Document GetDocument(object dictionary)
        {
            var instance = dictionary as IDictionary<TKey, TValue>;

            if (instance == null)
                return null;

            var doc = new Document();

            foreach (var e in instance)
                doc.Add(ValueConverter.ConvertKey(e.Key), e.Value);

            return doc;
        }

    }
}