using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDB.Driver.Linq
{
    public class MongoQuery<T> : IOrderedQueryable<T>, IOrderedQueryable, IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable
    {
        private readonly Expression _expression;
        private readonly MongoQueryProvider _provider;

        Expression IQueryable.Expression
        {
            get { return _expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof(T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return this._provider; }
        }

        public MongoQuery(MongoQueryProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            this._expression = Expression.Constant(this);
            this._provider = provider;
        }

        public MongoQuery(MongoQueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");
            this._provider = provider;
            this._expression = expression;
        }
 
        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)this._provider.Execute(_expression)).GetEnumerator();
        }
 
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this._provider.Execute(_expression)).GetEnumerator();
        }
 
        public override string ToString() {
            return _provider.GetQueryObject(_expression).ToString();
        }
    }
}
