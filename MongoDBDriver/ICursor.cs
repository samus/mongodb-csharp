using System;
using System.Collections.Generic;

namespace MongoDB.Driver {
    public interface ICursor<T> : IDisposable
    {
        long Id { get; }
        string FullCollectionName { get; }
        ICursor<T> Spec(Document spec);
        ICursor<T> Limit(int limit);
        ICursor<T> Skip(int skip);
        ICursor<T> Fields(Document fields);
        ICursor<T> Sort(string field);
        ICursor<T> Sort(string field, IndexOrder order);
        ICursor<T> Sort(Document fields);
        ICursor<T> Hint(Document index);
        ICursor<T> Snapshot(Document index);
        T Explain();
        bool Modifiable { get; }
        IEnumerable<T> Documents { get; }
    }
}
