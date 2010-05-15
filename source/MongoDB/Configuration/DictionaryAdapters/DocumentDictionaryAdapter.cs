using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Configuration.DictionaryAdapters
{
    public class DocumentDictionaryAdapter : IDictionaryAdapter
    {
        public object CreateDictionary(Type valueType, DictionaryEntry[] pairs)
        {
            var doc = new Document();

            foreach (var pair in pairs)
                doc.Add((string)pair.Key, pair.Value);

            return doc;
        }

        public IEnumerable<DictionaryEntry> GetPairs(object collection)
        {
            var doc = collection as Document;
            if (doc == null)
                return Enumerable.Empty<DictionaryEntry>();

            return doc.Select(x => new DictionaryEntry(x.Key, x.Value));
        }
    }
}