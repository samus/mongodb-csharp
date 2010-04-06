using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Driver.Linq
{
    internal class MongoQueryObject
    {
        private Stack<Scope> _scopes;

        /// <summary>
        /// Gets or sets the number to skip.
        /// </summary>
        /// <value>The number to skip.</value>
        public int NumberToSkip { get; set; }

        /// <summary>
        /// Gets or sets the number to limit.
        /// </summary>
        /// <value>The number to limit.</value>
        public int NumberToLimit { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public Document Order { get; set; }

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public MongoFieldProjection Projection { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public Document Query { get; set; }

        public MongoQueryObject()
        {
            Order = new Document();
            Query = new Document();

            _scopes = new Stack<Scope>();
        }

        public void AddCondition(object value)
        {
            _scopes.Peek().AddCondition(value);
        }

        public void PushConditionScope(string name)
        {
            if (_scopes.Count == 0)
                _scopes.Push(new Scope(name, Query[name]));
            else
                _scopes.Push(_scopes.Peek().CreateChildScope(name));
        }

        public void PopConditionScope()
        {
            var scope = _scopes.Pop();
            var doc = Query;
            foreach (var s in _scopes.Reverse()) //as if it were a queue
            {
                var sub = doc[s.Key];
                if (sub == null)
                    doc[s.Key] = sub = new Document();
                else if (!(sub is Document))
                    throw new InvalidQueryException();

                doc = (Document)sub;
            }

            doc[scope.Key] = scope.Value;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        private class Scope
        {
            public string Key { get; private set; }

            public object Value { get; private set; }

            public Scope(string key, object initialValue)
            {
                Key = key;
                Value = initialValue;
            }

            public void AddCondition(object value)
            {
                if (Value is Document)
                {
                    if (!(value is Document))
                        throw new InvalidQueryException();

                    ((Document)Value).Merge((Document)value);
                }
                else if (Value is Document)
                    throw new InvalidQueryException();
                else if(Value != null)
                    throw new InvalidQueryException();
                else
                    Value = value;
            }

            public Scope CreateChildScope(string name)
            {
                if (Value is Document)
                    return new Scope(name, ((Document)Value)[name]);

                return new Scope(name, null);
            }
        }
    }
}