using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Driver.Connections;
using MongoDB.Driver.IO;

namespace MongoDB.Driver
{
    public class Collection : IMongoCollection 
    {
        private static OidGenerator oidGenerator = new OidGenerator();
        
        private Connection connection;
        
        private string name;        
        public string Name {
            get { return name; }
        }
        
        private string dbName;      
        public string DbName {
            get { return dbName; }
        }
        
        public string FullName{
            get{ return dbName + "." + name;}
        }
        
        private CollectionMetaData metaData;        
        public CollectionMetaData MetaData {
            get { 
                if(metaData == null){
                    metaData = new CollectionMetaData(this.dbName,this.name, this.connection);
                }
                return metaData;
            }
        }

        private Database db;
        private Database Db{
            get{
                if(db == null)
                    db = new Database(this.connection, this.dbName);
                return db;
            }
        }
        public Collection(string name, Connection conn, string dbName)
        {
            this.name = name;
            this.connection = conn;
            this.dbName = dbName;
        }
        
        /// <summary>
        /// Finds and returns the first document in a query. 
        /// </summary>
        /// <param name="spec">
        /// A <see cref="Document"/> representing the query.
        /// </param>
        /// <returns>
        /// A <see cref="Document"/> from the collection.
        /// </returns>
        public Document FindOne(Document spec){
            ICursor cur = this.Find(spec, -1, 0, null);
            foreach(Document doc in cur.Documents){
                cur.Dispose();
                return doc;
            }
            //FIXME Decide if this should throw a not found exception instead of returning null.
            return null; //this.Find(spec, -1, 0, null)[0];
        }
        
        public ICursor FindAll() {
            Document spec = new Document();
            return this.Find(spec, 0, 0, null);
        }
        
        public ICursor Find(String where){
            Document spec = new Document();
            spec.Append("$where", new Code(where));
            return this.Find(spec, 0, 0, null);
        }
        
        public ICursor Find(Document spec) {
            return this.Find(spec, 0, 0, null);
        }
        
        public ICursor Find(Document spec, int limit, int skip) {
            return this.Find(spec, limit, skip, null);
        }
        
        public ICursor Find(Document spec, int limit, int skip, Document fields) {
            if(spec == null) spec = new Document();
            Cursor cur = new Cursor(connection, this.FullName, spec, limit, skip, fields);
            return cur;
        }
        
        /// <summary>
        /// Entrypoint into executing a map/reduce query against the collection. 
        /// </summary>
        /// <returns>
        /// A <see cref="MapReduce"/>
        /// </returns>
        public MapReduce MapReduce(){
            return new MapReduce(this.Db, this.Name);
        }
        
        public MapReduceBuilder MapReduceBuilder(){
            return new MapReduceBuilder(this.MapReduce());
        }
            
        
        /// <summary>
        ///Count all items in the collection. 
        /// </summary>
        public long Count(){
            return this.Count(new Document());
        }
        
        /// <summary>
        /// Count all items in a collection that match the query spec. 
        /// </summary>
        /// <remarks>
        /// It will return 0 if the collection doesn't exist yet.
        /// </remarks>
        public long Count(Document spec){
            try{
                //Database db = new Database(this.connection, this.dbName);
                Document ret = this.Db.SendCommand(new Document().Append("count",this.Name).Append("query",spec));
                double n = (double)ret["n"];
                return Convert.ToInt64(n);
            }catch(MongoCommandException){
                //FIXME This is an exception condition when the namespace is missing. 
                //-1 might be better here but the console returns 0.
                return 0;
            }
            
        }
        
        /// <summary>
        /// Inserts the Document into the collection. 
        /// </summary>
        public void Insert (Document doc, bool safemode){
            Insert(doc);
            CheckError(safemode);
        }

        public void Insert(Document doc){
            Document[] docs = new Document[]{doc,};
            this.Insert(docs);
        }
        
        public void Insert (IEnumerable<Document> docs, bool safemode){
            if(safemode)this.Db.ResetError();
            this.Insert(docs);
            CheckPreviousError(safemode);
        }

        public void Insert(IEnumerable<Document> docs){
            InsertMessage im = new InsertMessage();
            im.FullCollectionName = this.FullName;
            List<Document> idocs = new List<Document>();
            foreach(Document doc in docs){
                if(doc.Contains("_id") == false){
                    Oid _id = oidGenerator.Generate();
                    doc.Prepend("_id",_id);
                }
            }
            idocs.AddRange(docs);
            im.Documents = idocs.ToArray();
            try{
                this.connection.SendMessage(im);    
            }catch(IOException ioe){
                throw new MongoCommException("Could not insert document, communication failure", this.connection,ioe);
            }   
        }
        
        /// <summary>
        /// Deletes documents from the collection according to the spec.
        /// </summary>
        /// <remarks>An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        public void Delete (Document selector, bool safemode){
            Delete(selector);
            CheckError(safemode);
        }
        
        /// <summary>
        /// Deletes documents from the collection according to the spec.
        /// </summary>
        /// <remarks>An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        public void Delete(Document selector){
            DeleteMessage dm = new DeleteMessage();
            dm.FullCollectionName = this.FullName;
            dm.Selector = selector;
            try{
                this.connection.SendMessage(dm);
            }catch(IOException ioe){
                throw new MongoCommException("Could not delete document, communication failure", this.connection,ioe);
            }
        }
        
        
        public void Update (Document doc, bool safemode){
            Update(doc);
            CheckError(safemode);
        }
        
        /// <summary>
        /// Saves a document to the database using an upsert.
        /// </summary>
        /// <remarks>
        /// The document will contain the _id that is saved to the database.  This is really just an alias
        /// to Update(Document) to maintain consistency between drivers.
        /// </remarks>
        public void Save(Document doc){
            Update(doc);
        }
        
        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <remarks>
        /// _id will be used in the document to create a selector.  If it isn't in
        /// the document then it is assumed that the document is new and an upsert is sent to the database
        /// instead.
        /// </remarks>
        public void Update(Document doc){
            //Try to generate a selector using _id for an existing document.
            //otherwise just set the upsert flag to 1 to insert and send onward.
            Document selector = new Document();
            int upsert = 0;
            if(doc.Contains("_id")  & doc["_id"] != null){
                selector["_id"] = doc["_id"];   
            }else{
                //Likely a new document
                doc.Prepend("_id",oidGenerator.Generate());
                upsert = 1;
            }
            this.Update(doc, selector, upsert);
        }
        
        public void Update (Document doc, Document selector, bool safemode){
            Update(doc, selector,0,safemode);
        }
        
        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>        
        public void Update(Document doc, Document selector){
            this.Update(doc, selector, 0);
        }
        
        public void Update (Document doc, Document selector, UpdateFlags flags, bool safemode){
            Update(doc,selector,flags);
            CheckError(safemode);
        }
        
        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> to update with
        /// </param>
        /// <param name="selector">
        /// The query spec to find the document to update.
        /// </param>
        /// <param name="flags">
        /// <see cref="UpdateFlags"/>
        /// </param>
        public void Update(Document doc, Document selector, UpdateFlags flags){
            UpdateMessage um = new UpdateMessage();
            um.FullCollectionName = this.FullName;
            um.Selector = selector;
            um.Document = doc;
            um.Flags = (int)flags;
            try{
                this.connection.SendMessage(um);
            }catch(IOException ioe){
                throw new MongoCommException("Could not update document, communication failure", this.connection,ioe);
            }           
            
        }
        
        public void Update (Document doc, Document selector, int flags, bool safemode){
            Update(doc,selector,flags);
            CheckError(safemode);
        }
        
        public void Update(Document doc, Document selector, int flags){
            //TODO Update the interface and make a breaking change.
            this.Update(doc,selector,(UpdateFlags)flags);
        }
        
        /// <summary>
        /// Runs a multiple update query against the database.  It will wrap any 
        /// doc with $set if the passed in doc doesn't contain any '$' ops.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="selector"></param>
        public void UpdateAll(Document doc, Document selector){
            bool foundOp = false;
            foreach(string key in doc.Keys){
                if(key.IndexOf('$') == 0){
                    foundOp = true;
                    break;
                }
            }
            if(foundOp == false){
                //wrap document in a $set.
                Document s = new Document().Append("$set", doc);
                doc = s;
            }
            this.Update(doc, selector, UpdateFlags.MultiUpdate);           
        }
        
        
        public void UpdateAll (Document doc, Document selector, bool safemode){
            if(safemode)this.Db.ResetError();
            this.UpdateAll(doc, selector);
            CheckPreviousError(safemode);
        }
        

        private void CheckError(bool safemode){
            if(safemode){
                Document err = this.Db.GetLastError();
                if(ErrorTranslator.IsError(err)) throw ErrorTranslator.Translate(err);
            }
        }
        private void CheckPreviousError(bool safemode){
            if(safemode){
                Document err = this.Db.GetPreviousError();
                if(ErrorTranslator.IsError(err)) throw ErrorTranslator.Translate(err);
            }
        }
    }
}