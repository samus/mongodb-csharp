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
        /// Gets or sets the map function.
        /// </summary>
        /// <value>The map function.</value>
        public string MapFunction { get; set; }

        /// <summary>
        /// Gets or sets the reduce function.
        /// </summary>
        /// <value>The reduce function.</value>
        public string ReduceFunction { get; set; }

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

        public MongoQueryObject()
        {
            Fields = new Document();
            _query = new Document();

            _hasOrder = false;

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

        public void SetQueryDocument(Document document)
        {
            if (_hasOrder)
                _query["query"] = document;
            else
                _query = document;
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
    }
}