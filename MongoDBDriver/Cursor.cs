using System;
using System.Collections.Generic;
using System.IO;

using MongoDB.Driver.Connections;
using MongoDB.Driver.Generic;

namespace MongoDB.Driver
{
    public class Cursor : ICursor
    {
        private MongoDB.Driver.Generic.Cursor<Document> _cursor;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Cursor&lt;T&gt;" /> class.
        /// </summary>
        /// <param name = "connection">The conn.</param>
        /// <param name = "fullCollectionName">Full name of the collection.</param>
        public Cursor(Connection connection, string fullCollectionName){
            //Todo: should be internal
            _cursor = new Cursor<Document>(connection, fullCollectionName);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Cursor&lt;T&gt;" /> class.
        /// </summary>
        /// <param name = "connection">The conn.</param>
        /// <param name = "fullCollectionName">Full name of the collection.</param>
        [Obsolete("Use Cursor(Connection, fullCollectionName) and then call the Spec, Limit, Skip and Fields methods")]
        public Cursor(Connection connection, string fullCollectionName, Document spec, int limit, int skip, Document fields)
            : this(connection, fullCollectionName){
            //Todo: should be internal
            _cursor = new Cursor<Document>(connection, fullCollectionName, spec, limit, skip, fields);
        }

        /// <summary>
        /// Gets or sets the full name of the collection.
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { 
            get{ return _cursor.FullCollectionName;}
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public long Id { 
            get{ return _cursor.Id; }
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
        /// Limits the returned documents to the specified fields
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor Fields(Document fields){
            _cursor.Fields(fields);
            return this;
        }

        /// <summary>
        /// Sorts the specified ascending on the specified field name.
        /// </summary>
        /// <param name = "field">The field.</param>
        /// <returns></returns>
        public ICursor Sort(string field){
            _cursor.Sort(field);
            return this;
        }

        /// <summary>
        /// Sorts on the specified field.
        /// </summary>
        /// <param name = "field">The field.</param>
        /// <param name = "order">The order.</param>
        /// <returns></returns>
        public ICursor Sort(string field, IndexOrder order){
            _cursor.Sort(field, order);
            return this;
        }

        /// <summary>
        /// Document containing the fields to sort on and the order (ascending/descending)
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor Sort(Document fields){
            _cursor.Sort(fields);
            return this;
        }


        /// <summary>
        /// Hint to use the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ICursor Hint(Document index)
        {
            _cursor.Hint(index);
            return this;
        }

        /// <summary>
        /// Snapshot mode assures that objects which update during the lifetime of a query are returned once 
        /// and only once. This is most important when doing a find-and-update loop that changes the size of 
        /// documents that are returned ($inc does not change size).
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <remarks>Because snapshot mode traverses the _id index, it may not be used with sorting or 
        /// explicit hints. It also cannot use any other index for the query.</remarks>
        public ICursor Snapshot(){
            _cursor.Snapshot();
            return this;
        }

        /// <summary>
        ///   Explains this instance.
        /// </summary>
        /// <returns></returns>
        public Document Explain(){
            return _cursor.Explain();
        }
        
        /// <summary>
        ///   Optionses the specified options.
        /// </summary>
        /// <param name = "options">The options.</param>
        /// <returns></returns>
        public ICursor Options(QueryOptions options){
            _cursor.Options(options);
            return this;
        }
        
        /// <summary>
        ///   Gets a value indicating whether this <see cref = "Cursor&lt;T&gt;" /> is modifiable.
        /// </summary>
        /// <value><c>true</c> if modifiable; otherwise, <c>false</c>.</value>
        public bool IsModifiable{
            get { return _cursor.IsModifiable; }
        }

        /// <summary>
        ///   Gets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public IEnumerable<Document> Documents{
            get{
                foreach(var doc in _cursor.Documents){
                    yield return doc;
                }
            }
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){
            _cursor.Dispose();
        }
    }
}