using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MongoDB.Configuration.DictionaryAdapters
{
    public class GenericDictionaryDictionaryAdapter : IDictionaryAdapter
    {
        private static readonly Type OpenType = typeof(Dictionary<,>);

        /// <summary>
        /// Creates the dictionary.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="pairs">The pairs.</param>
        /// <returns></returns>
        public object CreateDictionary(Type valueType, DictionaryEntry[] pairs)
        {
            var closedType = OpenType.MakeGenericType(typeof(string), valueType);
            var instance = Activator.CreateInstance(closedType);
            var addMethod = closedType.GetMethod("Add", new [] { typeof(string), valueType });
            foreach(var pair in pairs)
                addMethod.Invoke(instance, new [] {pair.Key, pair.Value });

            return instance;
        }

        /// <summary>
        /// Gets the pairs.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public IEnumerable<DictionaryEntry> GetPairs(object collection)
        {
            throw new NotImplementedException();
        }
    }
}