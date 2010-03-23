using System;
using System.Collections.Generic;
using System.IO;

using MongoDB.Driver.Connections;

namespace MongoDB.Driver
{
    /// <summary>
    /// <see cref="ICursor"/>
    /// </summary>
    public class Cursor : ICursor
    {
        private ICursor<Document> _cursor;

        public long Id {
            get { return _cursor.Id; }
        }

        internal Cursor(ICursor<Document> cursor)
        {
            _cursor = cursor;
        }

        public ICursor Spec(Document spec){
            _cursor.Spec(spec);
            return this;
        }

        public ICursor Limit(int limit){
            _cursor.Limit(limit);
            return this;
        }


        public ICursor Skip(int skip){
            _cursor.Skip(skip);
            return this;
        }


        public ICursor Fields(Document fields){
            _cursor.Fields(fields);
            return this;
        }

        public ICursor Options(QueryOptions options){
            _cursor.Options(options);
            return this;
        }

        public ICursor Sort(string field){
            _cursor.Sort(field);
            return this;
        }


        public ICursor Sort(string field, IndexOrder order){
            _cursor.Sort(field, order);
            return this;
        }


        public ICursor Sort(Document fields){
            _cursor.Sort(fields);
            return this;
        }


        public ICursor Hint(Document index){
            _cursor.Hint(index);
            return this;
        }


        public ICursor Snapshot(){
            _cursor.Snapshot();
            return this;
        }


        public Document Explain(){
            return _cursor.Explain();
        }


        public bool IsModifiable {
            get { return _cursor.IsModifiable; }
        }


        public IEnumerable<Document> Documents {
            get {
                foreach (var doc in _cursor.Documents) {
                    yield return doc;
                }
            }
        }

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
