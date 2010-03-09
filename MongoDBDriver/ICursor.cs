using System;
using System.Collections.Generic;

namespace MongoDB.Driver {
    public interface ICursor : IDisposable {
        long Id { get; }
        string FullCollectionName { get; }
        ICursor Spec(IEnumerable<KeyValuePair<String, Object>> spec);
        ICursor Limit(int limit);
        ICursor Skip(int skip);
        ICursor Fields (IEnumerable<KeyValuePair<String, Object>> fields);
        ICursor Sort(string field);
        ICursor Sort(string field, IndexOrder order);
        ICursor Sort(IEnumerable<KeyValuePair<String, Object>> fields);
        ICursor Hint(IEnumerable<KeyValuePair<String, Object>> index);
        ICursor Snapshot(IEnumerable<KeyValuePair<String, Object>> index);
        Document Explain();
        bool Modifiable { get; }
        IEnumerable<Document> Documents { get; }
    }
}
