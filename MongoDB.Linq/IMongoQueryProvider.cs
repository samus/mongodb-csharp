using System.Linq;
using System.Linq.Expressions;

namespace MongoDB.Linq {
    public interface IMongoQueryProvider : IQueryProvider {
        MongoQuerySpec GetQuerySpec(Expression expression);
    }

}
