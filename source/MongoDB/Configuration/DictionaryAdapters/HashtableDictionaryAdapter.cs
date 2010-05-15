using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Configuration.DictionaryAdapters
{
    public class HashtableDictionaryAdapter : IDictionaryAdapter
    {
        public object CreateDictionary(Type valueType, DictionaryEntry[] pairs)
        {
            var hashtable = new Hashtable();

            foreach (var pair in pairs)
                hashtable.Add((string)pair.Key, pair.Value);

            return hashtable;
        }

        public IEnumerable<DictionaryEntry> GetPairs(object collection)
        {
            var hashtable = collection as Hashtable;
            if (hashtable == null)
                return Enumerable.Empty<DictionaryEntry>();

            return hashtable.OfType<DictionaryEntry>();
        }
    }
}