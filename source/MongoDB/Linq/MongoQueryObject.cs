using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Linq
{
    internal class MongoQueryObject
    {
        private bool _hasOrder;
        private Document _query;
        private readonly Stack<Scope> _scopes;

        /// <summary>
        /// Gets or sets the aggregator.
        /// </summary>
        /// <value>The aggregator.</value>
        public LambdaExpression Aggregator { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public IMongoDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets the type of the document.
        /// </summary>
        /// <value>The type of the document.</value>
        public Type DocumentType { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>The fields.</value>
        public Document Fields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a count query.
        /// </summary>
        /// <value><c>true</c> if this is a count query; otherwise, <c>false</c>.</value>
        public bool IsCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is map reduce.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is map reduce; otherwise, <c>false</c>.
        /// </value>
        public bool IsMapReduce { get; set; }

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
        /// Gets or sets the projector.
        /// </summary>
        /// <value>The projector.</value>
        public LambdaExpression Projector { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public Document Query
        {
            get { return _query; }
        }

        /// <summary>
        /// Gets the scope depth.
        /// </summary>
        /// <value>The scope depth.</value>
        public int ScopeDepth
        {
            get { return _scopes.Count; }
        }

        public MongoQueryObject()
        {
            Fields = new Document();
            _query = new Document();

            _hasOrder = false;

            _scopes = new Stack<Scope>();
        }

        public void AddCondition(object value)
        {
            _scopes.Peek().AddCondition(value);
        }

        public void AddCondition(string name, object value)
        {
            PushConditionScope(name);
            AddCondition(value);
            PopConditionScope();
        }

        public void AddOrderBy(string name, int value)
        {
            if (!_hasOrder)
            {
                _query = new Document("query", _query);
                _query.Add("orderby", new Document());
                _hasOrder = true;
            }

            ((Document)_query["orderby"]).Add(name, value);
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
            if (scope.Value == null)
                return;

            var doc = _query;
            if (_hasOrder)
                doc = (Document)doc["query"];
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

        public void SetWhereClause(string whereClause)
        {
            if (_hasOrder)
            {
                _query.Add("$where", new Code(whereClause));
                _query.Remove("query");
            }
            else
                _query = new Document("$where", new Code(whereClause));
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