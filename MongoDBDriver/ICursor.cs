using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver {
    public interface ICursor : IDisposable {
        long Id { get; }
        string FullCollectionName { get; }
        ICursor Spec(Document spec);
        ICursor Limit(int limit);
        ICursor Skip(int skip);
        ICursor Fields (Document fields);
        bool Modifiable { get; }
        IEnumerable<Document> Documents { get; }
    }
}
