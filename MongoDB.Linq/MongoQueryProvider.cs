using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MongoDB.Driver;

namespace MongoDB.Linq {
    public class MongoQueryProvider : IMongoQueryProvider {

        private struct Result {
            public IEnumerable<Document> Documents;
            public bool IsFirstCall;
        }

        private readonly IMongoCollection collection;

        public MongoQueryProvider(IMongoCollection collection)
        {
            this.collection = collection;
        }

        public IQueryable CreateQuery(Expression expression) {
            return new MongoQuery(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            return (IQueryable<TElement>)new MongoQuery(this, expression);
        }

        public object Execute(Expression expression) {
            return ExecuteInternal(expression).Documents;
        }

        private Result ExecuteInternal(Expression expression) {
            var spec = new MongoQueryTranslator().Translate(expression);
            var cur = collection.Find(spec.Query, spec.Limit, spec.Skip, spec.Fields);
            return new Result {
                Documents = cur.Documents,
                IsFirstCall = spec.IsFirstCall
            };
        }

        public TResult Execute<TResult>(Expression expression) {
            var result = ExecuteInternal(expression);
            if (typeof(TResult).IsAssignableFrom(typeof(Document))) {
                return (TResult)(object)((result.IsFirstCall) ? result.Documents.First() : result.Documents.FirstOrDefault());
            }
            return (TResult)result.Documents;
        }

        public MongoQuerySpec GetQuerySpec(Expression expression) {
            return new MongoQueryTranslator().Translate(expression);
        }
    }
}
