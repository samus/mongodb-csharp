using System.Linq;
using MongoDB.Driver;

namespace MongoDB.Linq {
    public interface IMongoQuery : IQueryable<Document> {
        Document Query { get; }
        int Limit { get; }
        int Skip { get; }
        Document Fields { get; }
    }
}
