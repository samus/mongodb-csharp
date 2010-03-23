using System;
using System.Collections.Generic;

namespace MongoDB.Driver{
    public interface ICursor<T> : IDisposable
    {
        long Id { get; }
        ICursor<T> Spec(Document spec);
        ICursor<T> Spec(object spec);
        ICursor<T> Limit(int limit);
        ICursor<T> Skip(int skip);
        ICursor<T> Fields(Document fields);
        ICursor<T> Fields(object fields);
        ICursor<T> Options(QueryOptions options);
        ICursor<T> Sort(string field);
        ICursor<T> Sort(string field, IndexOrder order);
        ICursor<T> Sort(Document fields);
        ICursor<T> Sort(object fields);
        ICursor<T> Hint(Document index);
        ICursor<T> Hint(object index);
        ICursor<T> Snapshot();
        Document Explain();
        bool IsModifiable { get; }
        IEnumerable<T> Documents { get; }
    }
}
