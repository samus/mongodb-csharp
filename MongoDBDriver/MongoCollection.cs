using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Driver.Connections;
using MongoDB.Driver.Protocol;

namespace MongoDB.Driver
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoCollection<T> : IMongoCollection<T>
        where T : class
    {
        private static readonly OidGenerator OidGenerator = new OidGenerator();
        private readonly Connection _connection;
        private Database _database;
        private CollectionMetaData _metaData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="name">The name.</param>
        public MongoCollection(Connection connection, string databaseName, string name){
            //Todo: This should be internal
            Name = name;
            DatabaseName = databaseName;
            _connection = connection;
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public Database Database{
            get { return _database ?? (_database = new Database(_connection, DatabaseName)); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the full name including database name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName{
            get { return DatabaseName + "." + Name; }
        }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        public CollectionMetaData MetaData{
            get { return _metaData ?? (_metaData = new CollectionMetaData(DatabaseName, Name, _connection)); }
        }

        /// <summary>
        /// Finds and returns the first document in a query.
        /// </summary>
        /// <param name="spec">A <see cref="Document"/> representing the query.</param>
        /// <returns>
        /// A <see cref="Document"/> from the collection.
        /// </returns>
        public T FindOne(Document spec)
        {
            return FindOne((object)spec);
        }

        /// <summary>
        /// Finds and returns the first document in a query.
        /// </summary>
        /// <param name="spec">A <see cref="Document"/> representing the query.</param>
        /// <returns>
        /// A <see cref="Document"/> from the collection.
        /// </returns>
        public T FindOne(object spec)
        {
            var cursor = Find(spec, -1, 0, null);
            foreach(var doc in cursor.Documents)
            {
                cursor.Dispose();
                return doc;
            }
            //FIXME Decide if this should throw a not found exception instead of returning null.
            return null; //this.Find(spec, -1, 0, null)[0];
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <returns></returns>
        public ICursor<T> FindAll()
        {
            var spec = new Document();
            return Find(spec, 0, 0, null);
        }

        /// <summary>
        /// Finds the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public ICursor<T> Find(String where)
        {
            var spec = new Document();
            spec.Append("$where", new Code(where));
            return Find(spec, 0, 0, null);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public ICursor<T> Find(Document spec)
        {
            return Find((object)spec);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public ICursor<T> Find(object spec)
        {
            return Find(spec, 0, 0, null);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        public ICursor<T> Find(Document spec, int limit, int skip)
        {
            return Find((object)spec, limit, skip, null);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        public ICursor<T> Find(object spec, int limit, int skip)
        {
            return Find(spec, limit, skip, null);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Find(Document spec, int limit, int skip, Document fields)
        {
            return Find((object)spec, limit, skip, (object)fields);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ICursor<T> Find(object spec, int limit, int skip, object fields)
        {
            if(spec == null)
                spec = new Document();
            return new Cursor<T>(_connection, FullName, spec, limit, skip, fields);
        }

        /// <summary>
        /// Entrypoint into executing a map/reduce query against the collection.
        /// </summary>
        /// <returns>A <see cref="MapReduce"/></returns>
        public MapReduce MapReduce(){
            return new MapReduce(Database, Name);
        }

        /// <summary>
        /// Maps the reduce builder.
        /// </summary>
        /// <returns></returns>
        public MapReduceBuilder MapReduceBuilder(){
            return new MapReduceBuilder(MapReduce());
        }

        ///<summary>
        ///  Count all items in the collection.
        ///</summary>
        public long Count(){
            return Count(new Document());
        }

        /// <summary>
        /// Count all items in a collection that match the query spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        /// <remarks>
        /// It will return 0 if the collection doesn't exist yet.
        /// </remarks>
        public long Count(Document spec){
            return Count((object)spec);
        }

        /// <summary>
        /// Count all items in a collection that match the query spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        /// <remarks>
        /// It will return 0 if the collection doesn't exist yet.
        /// </remarks>
        public long Count(object spec)
        {
            try
            {
                var response = Database.SendCommand(new Document().Append("count", Name).Append("query", spec));
                return Convert.ToInt64((double)response["n"]);
            }
            catch(MongoCommandException)
            {
                //FIXME This is an exception condition when the namespace is missing. 
                //-1 might be better here but the console returns 0.
                return 0;
            }
        }

        /// <summary>
        ///   Inserts the Document into the collection.
        /// </summary>
        public void Insert(Document document, bool safemode){
            Insert((object)document,safemode);
        }

        /// <summary>
        ///   Inserts the Document into the collection.
        /// </summary>
        public void Insert(object document, bool safemode)
        {
            Insert(document);
            CheckError(safemode);
        }

        /// <summary>
        /// Inserts the specified doc.
        /// </summary>
        /// <param name="document">The doc.</param>
        public void Insert(Document document){
            Insert((object)document);
        }

        /// <summary>
        /// Inserts the specified doc.
        /// </summary>
        /// <param name="document">The doc.</param>
        public void Insert(object document)
        {
            Insert(new[] { document });
        }

        /// <summary>
        /// Inserts the specified documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Insert(IEnumerable<Document> documents, bool safemode)
        {
            Insert((IEnumerable<object>)documents, safemode);
        }

        /// <summary>
        /// Inserts the specified documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Insert<TElement>(IEnumerable<TElement> documents, bool safemode)
        {
            if(safemode)
                Database.ResetError();
            Insert(documents);
            CheckPreviousError(safemode);
        }

        /// <summary>
        /// Inserts the specified documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        public void Insert(IEnumerable<Document> documents){
            Insert((IEnumerable<object>)documents);
        }

        /// <summary>
        /// Inserts the specified documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        public void Insert<TElement>(IEnumerable<TElement> documents)
        {
            var insertMessage = new InsertMessage
            {
                FullCollectionName = FullName
            };

            var insertDocument = new List<object>();

            //Fixme: This should be handled by an external class
            /*
            foreach(var doc in documents)
                if(doc.Contains("_id") == false)
                {
                    doc.Prepend("_id", OidGenerator.Generate());
                }
            */
            foreach(var document in documents)
                insertDocument.Add(document);

            insertMessage.Documents = insertDocument.ToArray();

            try
            {
                _connection.SendMessage(insertMessage);
            }
            catch(IOException exception)
            {
                throw new MongoCommException("Could not insert document, communication failure", _connection, exception);
            }
        }

        /// <summary>
        /// Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        public void Delete(Document selector, bool safemode){
            Delete((object)selector, safemode);
        }

        /// <summary>
        /// Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        public void Delete(object selector, bool safemode)
        {
            Delete(selector);
            CheckError(safemode);
        }

        /// <summary>
        /// Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        public void Delete(Document selector){
            Delete((object)selector);
        }

        /// <summary>
        /// Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        public void Delete(object selector)
        {
            var deleteMessage = new DeleteMessage
            {
                FullCollectionName = FullName,
                Selector = selector
            };

            try
            {
                _connection.SendMessage(deleteMessage);
            }
            catch(IOException exception)
            {
                throw new MongoCommException("Could not delete document, communication failure", _connection, exception);
            }
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(Document document, bool safemode){
            Update((object)document,safemode);
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(object document, bool safemode)
        {
            Update(document);
            CheckError(safemode);
        }

        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <remarks>
        /// _id will be used in the document to create a selector.  If it isn't in
        /// the document then it is assumed that the document is new and an upsert is sent to the database
        /// instead.
        /// </remarks>
        public void Update(Document document)
        {
            Update((object)document);
        }

        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <remarks>
        /// _id will be used in the document to create a selector.  If it isn't in
        /// the document then it is assumed that the document is new and an upsert is sent to the database
        /// instead.
        /// </remarks>
        public void Update(object document)
        {
            //Try to generate a selector using _id for an existing document.
            //otherwise just set the upsert flag to 1 to insert and send onward.
            var selector = new Document();
            var upsert = UpdateFlags.Upsert;

            //Todo: this needs to be cheched in an external class
            /*
             if(document.Contains("_id") & document["_id"] != null)
                selector["_id"] = document["_id"];
            else
            {
                //Likely a new document
                document.Prepend("_id", OidGenerator.Generate());
                upsert = UpdateFlags.Upsert;
            }
             * */

            Update(document, selector, upsert);
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(Document document, Document selector, bool safemode)
        {
            Update((object)document, (object)document, safemode);
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(object document, object selector, bool safemode)
        {
            Update(document, selector, 0, safemode);
        }

        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public void Update(Document document, Document selector)
        {
            Update((object)document, (object)selector);
        }
        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public void Update(object document, object selector)
        {
            Update(document, selector, 0);
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(Document document, Document selector, UpdateFlags flags, bool safemode)
        {
            Update((object)document, (object)selector, flags, safemode);
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(object document, object selector, UpdateFlags flags, bool safemode)
        {
            Update(document, selector, flags);
            CheckError(safemode);
        }

        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to update with</param>
        /// <param name="selector">The query spec to find the document to update.</param>
        /// <param name="flags"><see cref="UpdateFlags"/></param>
        public void Update(Document document, Document selector, UpdateFlags flags)
        {
            Update((object)document, (object)selector, flags);
        }
        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to update with</param>
        /// <param name="selector">The query spec to find the document to update.</param>
        /// <param name="flags"><see cref="UpdateFlags"/></param>
        public void Update(object document, object selector, UpdateFlags flags)
        {
            var updateMessage = new UpdateMessage
            {
                FullCollectionName = FullName,
                Selector = selector,
                Document = document,
                Flags = (int)flags
            };

            try
            {
                _connection.SendMessage(updateMessage);
            }
            catch(IOException exception)
            {
                throw new MongoCommException("Could not update document, communication failure", _connection, exception);
            }
        }

        /// <summary>
        /// Runs a multiple update query against the database.  It will wrap any
        /// doc with $set if the passed in doc doesn't contain any '$' ops.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public void UpdateAll(Document document, Document selector)
        {
            UpdateAll((object)document, (object)selector);
        }

        /// <summary>
        /// Runs a multiple update query against the database.  It will wrap any
        /// doc with $set if the passed in doc doesn't contain any '$' ops.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public void UpdateAll(object document, object selector)
        {
            var foundOp = false;

            //Todo: need to get this info external
            /*
             * foreach(string key in document.Keys)
                if(key.IndexOf('$') == 0)
                {
                    foundOp = true;
                    break;
                }*/

            if(foundOp == false)
            {
                //wrap document in a $set.
                document = new Document().Append("$set", document);
            }

            Update(document, selector, UpdateFlags.MultiUpdate);
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void UpdateAll(Document document, Document selector, bool safemode)
        {
            UpdateAll((object)document, (object)selector);
        }
        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void UpdateAll(object document, object selector, bool safemode)
        {
            if(safemode)
                Database.ResetError();
            UpdateAll(document, selector);
            CheckPreviousError(safemode);
        }

        /// <summary>
        /// Saves a document to the database using an upsert.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <remarks>
        /// The document will contain the _id that is saved to the database.  This is really just an alias
        /// to Update(Document) to maintain consistency between drivers.
        /// </remarks>
        public void Save(Document document)
        {
            Save((object)document);
        }
        /// <summary>
        /// Saves a document to the database using an upsert.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <remarks>
        /// The document will contain the _id that is saved to the database.  This is really just an alias
        /// to Update(Document) to maintain consistency between drivers.
        /// </remarks>
        public void Save(object document)
        {
            Update(document);
        }

        /// <summary>
        /// Checks the error.
        /// </summary>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        private void CheckError(bool safemode){
            if(!safemode)
                return;

            var lastError = Database.GetLastError();
            
            if(ErrorTranslator.IsError(lastError))
                throw ErrorTranslator.Translate(lastError);
        }

        /// <summary>
        /// Checks the previous error.
        /// </summary>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        private void CheckPreviousError(bool safemode){
            if(!safemode)
                return;

            var previousError = Database.GetPreviousError();
            
            if(ErrorTranslator.IsError(previousError))
                throw ErrorTranslator.Translate(previousError);
        }
    }
}