using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Configuration.DictionaryAdapters
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericDictionaryDictionaryAdapter : IDictionaryAdapter
    {
        private static readonly Type OpenType = typeof(Dictionary<,>);

        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public object CreateDictionary(Type valueType, Document document)
        {
            var closedType = OpenType.MakeGenericType(typeof(string), valueType);
            var instance = Activator.CreateInstance(closedType);
            var addMethod = closedType.GetMethod("Add", new [] { typeof(string), valueType });
            foreach (var pair in document)
                addMethod.Invoke(instance, new [] {pair.Key, pair.Value });

            return instance;
        }

        /// <summary>
        /// Gets the pairs.
        /// </summary>
        /// <param name="dictionary">The collection.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns></returns>
        public Document GetDocument(object dictionary, Type valueType)
        {
            var type = typeof(KeyValuePair<,>).MakeGenericType(typeof(string), valueType);
            var keyProperty = type.GetProperty("Key");
            var valueProperty = type.GetProperty("Value");

            if(dictionary==null)
                return null;

            var doc = new Document();
            foreach (var e in (IEnumerable)dictionary)
                doc.Add(keyProperty.GetValue(e, null).ToString(), valueProperty.GetValue(e, null));

            return doc;
        }
    }
}