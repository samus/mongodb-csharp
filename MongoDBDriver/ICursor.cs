using System;
using System.Collections.Generic;

namespace MongoDB.Driver {
    public interface ICursor : IDisposable {
        long Id { get; }
        string FullCollectionName { get; }
        ICursor Spec(Document spec);
        ICursor Limit(int limit);
        ICursor Skip(int skip);
        ICursor Fields (Document fields);
        ICursor Sort(string field);
        ICursor Sort(string field, IndexOrder order);
        ICursor Sort(Document fields);
        ICursor Hint(Document index);
        ICursor Snapshot(Document index);
        Document Explain();
        bool Modifiable { get; }
        IEnumerable<Document> Documents { get; }
    }
}
