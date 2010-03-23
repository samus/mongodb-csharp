using System;
using System.Collections.Generic;
using System.IO;

using MongoDB.Driver.Connections;
using MongoDB.Driver.Protocol;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver
{
    /// <summary>
    /// <see cref = "IMongoCollection" />
    /// </summary>
    public class MongoCollection : IMongoCollection
    {
        MongoCollection<Document> _collection;

        public string Database {
            get { return _collection.DatabaseName; }
        }
        public string Name {
            get { return _collection.Name; }
        }


        public string DatabaseName {
            get { return _collection.DatabaseName; }
        }


        public string FullName {
            get { return _collection.FullName; }
        }


        public CollectionMetaData MetaData {
            get { return _collection.MetaData; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="name">The name.</param>
        public MongoCollection(ISerializationFactory serializationFactory, Connection connection, string databaseName, string name)
        {
            _collection = new MongoCollection<Document>(serializationFactory, connection, databaseName, name);
        }

        public Document FindOne(Document spec){
            return _collection.FindOne(spec);
        }

        public ICursor FindAll(){
            return new Cursor(_collection.FindAll());
        }

        public ICursor Find(string @where){
            return new Cursor(_collection.Find(@where));
        }

        public ICursor Find(Document spec){
            return new Cursor(_collection.Find(spec));
        }

        public ICursor Find(Document spec, int limit, int skip){
            return new Cursor(_collection.Find(spec, limit, skip));
        }

        public ICursor Find(Document spec, int limit, int skip, Document fields){
            return new Cursor(_collection.Find(spec, limit, skip, fields));
        }

        public MapReduce MapReduce(){
            return _collection.MapReduce();
        }

        public MapReduceBuilder MapReduceBuilder(){
            return _collection.MapReduceBuilder();
        }

        public long Count(){
            return _collection.Count();
        }

        public long Count(Document spec){
            return _collection.Count(spec);
        }

        public void Insert(Document doc){
            _collection.Insert(doc);
        }

        public void Insert(Document doc, bool safemode){
            _collection.Insert(doc, safemode);
        }


        public void Insert(IEnumerable<Document> docs){
            _collection.Insert(docs);
        }

        public void Insert(IEnumerable<Document> docs, bool safemode){
            _collection.Insert(docs, safemode);
        }

        public void Delete(Document selector){
            _collection.Delete(selector);
        }

        public void Delete(Document selector, bool safemode){
            _collection.Delete(selector, safemode);
        }

        #region "Updates"
        
        public void Update(Document doc, bool safemode){
            _collection.Update(doc,safemode);
        }
        
        public void Update(Document doc){
            _collection.Update(doc);
        }

        public void Update(Document doc, Document selector){
            _collection.Update(doc,selector);
        }
        
        public void Update(Document doc, Document selector, bool safemode){
            _collection.Update(doc,selector,safemode);
        }

        public void Update(Document doc, Document selector, UpdateFlags flags){
            _collection.Update(doc,selector,flags);
        }

        public void Update(Document doc, Document selector, UpdateFlags flags, bool safemode){
            _collection.Update(doc,selector,flags,safemode);
        }

        public void UpdateAll(Document doc, Document selector){
            _collection.UpdateAll(doc,selector);
        }

        public void UpdateAll(Document doc, Document selector, bool safemode){
            _collection.UpdateAll(doc,selector,safemode);
        }
        #endregion
        
        public void Save(Document doc){
            _collection.Save(doc);
        }
                
    }
}
