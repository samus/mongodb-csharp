using System.Collections.Generic;

namespace MongoDB.Driver
{
    /// <summary>
    /// <see cref="ICursor"/>
    /// </summary>
    public class Cursor : ICursor
    {
        private readonly ICursor<Document> _cursor;
        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        internal Cursor(ICursor<Document> cursor)
        {
            _cursor = cursor;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public long Id
        {
            get { return _cursor.Id; }
        }

        /// <summary>
        /// Specs the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public ICursor Spec(Document spec){
            _cursor.Spec(spec);
            return this;
        }

        /// <summary>
        /// Limits the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public ICursor Limit(int limit){
            _cursor.Limit(limit);
            return this;
        }

        /// <summary>
        /// Skips the specified skip.
        /// </summary>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        public ICursor Skip(int skip){
            _cursor.Skip(skip);
            return this;
        }

        /// <summary>
        /// Fieldses the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor Fields(Document fields){
            _cursor.Fields(fields);
            return this;
        }

        /// <summary>
        /// Optionses the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public ICursor Options(QueryOptions options){
            _cursor.Options(options);
            return this;
        }

        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public ICursor Sort(string field){
            _cursor.Sort(field);
            return this;
        }

        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public ICursor Sort(string field, IndexOrder order){
            _cursor.Sort(field, order);
            return this;
        }

        /// <summary>
        /// Sorts the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor Sort(Document fields){
            _cursor.Sort(fields);
            return this;
        }

        /// <summary>
        /// Hints the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ICursor Hint(Document index){
            _cursor.Hint(index);
            return this;
        }

        /// <summary>
        /// Snapshots this instance.
        /// </summary>
        /// <returns></returns>
        public ICursor Snapshot(){
            _cursor.Snapshot();
            return this;
        }

        /// <summary>
        /// Explains this instance.
        /// </summary>
        /// <returns></returns>
        public Document Explain(){
            return _cursor.Explain();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is modifiable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modifiable; otherwise, <c>false</c>.
        /// </value>
        public bool IsModifiable {
            get { return _cursor.IsModifiable; }
        }

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public IEnumerable<Document> Documents {
            get {return _cursor.Documents;}
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){
            _cursor.Dispose();
        }
    }
}

//        /// <summary>
//        ///   Initializes a new instance of the <see cref = "Cursor&lt;T&gt;" /> class.
//        /// </summary>
//        /// <param name = "connection">The conn.</param>
//        /// <param name = "fullCollectionName">Full name of the collection.</param>
//        public Cursor(Connection connection, string fullCollectionName):base(connection, fullCollectionName){
//
//        }
//
//        /// <summary>
//        ///   Initializes a new instance of the <see cref = "Cursor&lt;T&gt;" /> class.
//        /// </summary>
//        /// <param name = "connection">The conn.</param>
//        /// <param name = "fullCollectionName">Full name of the collection.</param>
//        [Obsolete("Use Cursor(Connection, fullCollectionName) and then call the Spec, Limit, Skip and Fields methods")]
//        public Cursor(Connection connection, string fullCollectionName, Document spec, int limit, int skip, Document fields)
//            :base(connection, fullCollectionName, spec, limit, skip, fields){
//        }
