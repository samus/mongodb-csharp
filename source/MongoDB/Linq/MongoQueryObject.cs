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
        private Document _sort;

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
        /// Gets or sets the finalizer function.
        /// </summary>
        /// <value>The finalizer function.</value>
        public string FinalizerFunction { get; set; }

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

        /// <summary>
        /// Gets the sort.
        /// </summary>
        /// <value>The sort.</value>
        public Document Sort
        {
            get { return _sort; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoQueryObject"/> class.
        /// </summary>
        public MongoQueryObject()
        {
            Fields = new Document();
            _query = new Document();
        }

        /// <summary>
        /// Adds the sort.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddSort(string name, int value)
        {
            if(_sort == null)
                _sort = new Document();
            _sort.Add(name, value);
        }

        /// <summary>
        /// Sets the query document.
        /// </summary>
        /// <param name="document">The document.</param>
        public void SetQueryDocument(Document document)
        {
            _query = document;
        }

        /// <summary>
        /// Sets the where clause.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        public void SetWhereClause(string whereClause)
        {
            _query = Op.Where(whereClause);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}