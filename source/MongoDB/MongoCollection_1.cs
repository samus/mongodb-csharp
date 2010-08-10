using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Configuration;
using MongoDB.Connections;
using MongoDB.Protocol;
using MongoDB.Results;
using MongoDB.Util;
using MongoDB.Configuration.Mapping.Model;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoCollection<T> : IMongoCollection<T> where T : class
    {
        private readonly MongoConfiguration _configuration;
        private readonly Connection _connection;
        private MongoDatabase _database;
        private CollectionMetadata _metadata;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">The name.</param>
        internal MongoCollection(MongoConfiguration configuration, Connection connection, string databaseName, string collectionName)
        {
            //Todo: add public constructors for users to call
            Name = collectionName;
            DatabaseName = databaseName;
            _configuration = configuration;
            _connection = connection;
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public IMongoDatabase Database {
            get { return _database ?? (_database = new MongoDatabase(_configuration, _connection, DatabaseName)); }
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
        public string FullName {
            get { return DatabaseName + "." + Name; }
        }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        public CollectionMetadata Metadata {
            get { return _metadata ?? (_metadata = new CollectionMetadata(_configuration, DatabaseName, Name, _connection)); }
        }

        /// <summary>
        /// Finds and returns the first document in a selector query.
        /// </summary>
        /// <param name="javascriptWhere">The where.</param>
        /// <returns>
        /// A <see cref="Document"/> from the collection.
        /// </returns>
        public T FindOne(string javascriptWhere)
        {
            var spec = new Document { { "$where", new Code(javascriptWhere) } };
            using(var cursor = Find(spec, -1, 0, null))
                return cursor.Documents.FirstOrDefault();
        }

        /// <summary>
        /// Finds and returns the first document in a query.
        /// </summary>
        /// <param name="spec">A <see cref="Document"/> representing the query.</param>
        /// <returns>
        /// A <see cref="Document"/> from the collection.
        /// </returns>
        public T FindOne(object spec){
            using(var cursor = Find(spec, -1, 0, null))
                return cursor.Documents.FirstOrDefault();
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <returns></returns>
        public ICursor<T> FindAll(){
            var spec = new Document();
            return Find(spec, 0, 0, null);
        }

        /// <summary>
        /// Finds the specified where.
        /// </summary>
        /// <param name="javascriptWhere">The where.</param>
        /// <returns></returns>
        public ICursor<T> Find(string javascriptWhere){
            var spec = new Document { { "$where", new Code(javascriptWhere) } };
            return Find(spec, 0, 0, null);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public ICursor<T> Find(object spec){
            return Find(spec, 0, 0, null);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="fields"></param>
        /// <returns>A <see cref="ICursor"/></returns>
        public ICursor<T> Find(object spec, object fields){
            return Find(spec, 0, 0, fields);
        }

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        public ICursor<T> Find(object spec, int limit, int skip){
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
        public ICursor<T> Find(object spec, int limit, int skip, object fields){
            if (spec == null)
                spec = new Document();
            return new Cursor<T>(_configuration.SerializationFactory, _configuration.MappingStore, _connection, DatabaseName, Name, spec, limit, skip, fields);
        }

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="spec"><see cref="Document"/> to find the document.</param>
        /// <returns>A <see cref="Document"/></returns>
        public T FindAndModify(object document, object spec){
            return FindAndModify(document, spec, false);
        }

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="spec"><see cref="Document"/> to find the document.</param>
        /// <param name="sort"><see cref="Document"/> containing the names of columns to sort on with the values being the</param>
        /// <returns>A <see cref="Document"/></returns>
        /// <see cref="IndexOrder"/>
        public T FindAndModify(object document, object spec, object sort){
            return FindAndModify(document, spec, sort);
        }

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="spec"><see cref="Document"/> to find the document.</param>
        /// <param name="returnNew">if set to <c>true</c> [return new].</param>
        /// <returns>A <see cref="Document"/></returns>
        public T FindAndModify(object document, object spec, bool returnNew){
            return FindAndModify(document, spec, new Document(), returnNew);
        }

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="spec"><see cref="Document"/> to find the document.</param>
        /// <param name="sort"><see cref="Document"/> containing the names of columns to sort on with the values being the
        /// <see cref="IndexOrder"/></param>
        /// <param name="returnNew">if set to <c>true</c> [return new].</param>
        /// <returns>A <see cref="Document"/></returns>
        public T FindAndModify(object document, object spec, object sort, bool returnNew){
            try
            {
                var command = new Document
                {
                    {"findandmodify", Name},
                    {"query", spec},
                    {"update", EnsureUpdateDocument(document)},
                    {"sort", sort},
                    {"new", returnNew}
                };

                var response = _connection.SendCommand<FindAndModifyResult<T>>(_configuration.SerializationFactory,
                    DatabaseName,
                    typeof(T),
                    command);

                return response.Value;
            }
            catch(MongoCommandException)
            {
                // This is when there is no document to operate on
                return null;
            }
        }

        /// <summary>
        /// Entrypoint into executing a map/reduce query against the collection.
        /// </summary>
        /// <returns>A <see cref="MapReduce"/></returns>
        public MapReduce MapReduce(){
            return new MapReduce(Database, Name, typeof(T));
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
        public long Count(object spec){
            try {
                var response = Database.SendCommand(typeof(T),new Document().Add("count", Name).Add("query", spec));
                return Convert.ToInt64((double)response["n"]);
            } catch (MongoCommandException) {
                //FIXME This is an exception condition when the namespace is missing. 
                //-1 might be better here but the console returns 0.
                return 0;
            }
        }

        /// <summary>
        ///   Inserts the Document into the collection.
        /// </summary>
        public void Insert(object document, bool safemode){
            Insert(document);
            CheckError(safemode);
        }

        /// <summary>
        /// Inserts the specified doc.
        /// </summary>
        /// <param name="document">The doc.</param>
        public void Insert(object document){
            Insert(new[] { document });
        }

        /// <summary>
        /// Inserts all.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="documents">The documents.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Insert<TElement>(IEnumerable<TElement> documents, bool safemode){
            if (safemode)
                Database.ResetError();
            Insert(documents);
            CheckPreviousError(safemode);
        }

        /// <summary>
        /// Inserts the specified documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        public void Insert<TElement>(IEnumerable<TElement> documents){
            if(documents is Document)
            {
                Insert(new[]{(Document)documents});
                return;
            }

            var rootType = typeof(T);
            var writerSettings = _configuration.SerializationFactory.GetBsonWriterSettings(rootType);

            var insertMessage = new InsertMessage(writerSettings)
            {
                FullCollectionName = FullName
            };

            var descriptor = _configuration.SerializationFactory.GetObjectDescriptor(rootType);
            var insertDocument = new List<object>();
            
            foreach (var document in documents) {
                var id = descriptor.GetPropertyValue(document, "_id");
                
                if (id == null)
                    descriptor.SetPropertyValue(document, "_id", descriptor.GenerateId(document));
                
                insertDocument.Add(document);
            }
            
            insertMessage.Documents = insertDocument.ToArray();
            
            try {
                _connection.SendMessage(insertMessage,DatabaseName);
            } catch (IOException exception) {
                throw new MongoConnectionException("Could not insert document, communication failure", _connection, exception);
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
        [Obsolete("Use Remove instead")]
        public void Delete(object selector, bool safemode)
        {
            Delete(selector);
            CheckError(safemode);
        }

        /// <summary>
        /// Remove documents from the collection according to the selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// See the safemode description in the class description
        /// </remarks>
        public void Remove(object selector, bool safemode){
            Remove(selector);
            CheckError(safemode);
        }

        /// <summary>
        /// Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        [Obsolete("Use Remove instead")]
        public void Delete(object selector){
            var writerSettings = _configuration.SerializationFactory.GetBsonWriterSettings(typeof(T));

            try {
                _connection.SendMessage(new DeleteMessage(writerSettings)
                {
                    FullCollectionName = FullName,
                    Selector = selector
                },DatabaseName);
            } catch (IOException exception) {
                throw new MongoConnectionException("Could not delete document, communication failure", _connection, exception);
            }
        }

        /// <summary>
        /// Remove documents from the collection according to the selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        public void Remove(object selector){
            var writerSettings = _configuration.SerializationFactory.GetBsonWriterSettings(typeof(T));

            try
            {
                _connection.SendMessage(new DeleteMessage(writerSettings)
                {
                    FullCollectionName = FullName,
                    Selector = selector
                }, DatabaseName);
            }
            catch(IOException exception)
            {
                throw new MongoConnectionException("Could not delete document, communication failure", _connection, exception);
            }
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        [Obsolete("Use Save instead")]
        public void Update(object document, bool safemode)
        {
            Save(document, safemode);
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
        [Obsolete("Use Save(Document)")]
        public void Update(object document){
            Save(document);
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(object document, object selector, bool safemode){
            Update(document, selector, 0, safemode);
        }

        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public void Update(object document, object selector){
            Update(document, selector, 0);
        }

        /// <summary>
        /// Updates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void Update(object document, object selector, UpdateFlags flags, bool safemode){
            Update(document, selector, flags);
            CheckError(safemode);
        }

        /// <summary>
        /// Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to update with</param>
        /// <param name="selector">The query spec to find the document to update.</param>
        /// <param name="flags"><see cref="UpdateFlags"/></param>
        public void Update(object document, object selector, UpdateFlags flags){
            var writerSettings = _configuration.SerializationFactory.GetBsonWriterSettings(typeof(T));

            try {
                _connection.SendMessage(new UpdateMessage(writerSettings)
                {
                    FullCollectionName = FullName,
                    Selector = selector,
                    Document = document,
                    Flags = (int)flags
                }, DatabaseName);
            } catch (IOException exception) {
                throw new MongoConnectionException("Could not update document, communication failure", _connection, exception);
            }
        }

        /// <summary>
        /// Runs a multiple update query against the database.  It will wrap any
        /// doc with $set if the passed in doc doesn't contain any '$' ops.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        public void UpdateAll(object document, object selector){
            Update(EnsureUpdateDocument(document), selector, UpdateFlags.MultiUpdate);
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        public void UpdateAll(object document, object selector, bool safemode){
            if (safemode)
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
        public void Save(object document){
            //Try to generate a selector using _id for an existing document.
            //otherwise just set the upsert flag to 1 to insert and send onward.

            var descriptor = _configuration.SerializationFactory.GetObjectDescriptor(typeof(T));

            var value = descriptor.GetPropertyValue(document, "_id");

            if(value == null)
            {
                //Likely a new document
                descriptor.SetPropertyValue(document, "_id", descriptor.GenerateId(value));

                Insert(document);
            }
            else
                Update(document, new Document("_id", value), UpdateFlags.Upsert);
        }

        /// <summary>
        /// Saves a document to the database using an upsert.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        /// The document will contain the _id that is saved to the database.  This is really just an alias
        /// to Update(Document) to maintain consistency between drivers.
        /// </remarks>
        public void Save(object document, bool safemode)
        {
            Save(document);
            CheckError(safemode);
        }

        /// <summary>
        /// Checks the error.
        /// </summary>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        private void CheckError(bool safemode){
            if (!safemode)
                return;
            
            var lastError = Database.GetLastError();
            
            if (ErrorTranslator.IsError(lastError))
                throw ErrorTranslator.Translate(lastError);
        }

        /// <summary>
        /// Checks the previous error.
        /// </summary>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        private void CheckPreviousError(bool safemode){
            if (!safemode)
                return;
            
            var previousError = Database.GetPreviousError();
            
            if (ErrorTranslator.IsError(previousError))
                throw ErrorTranslator.Translate(previousError);
        }

        /// <summary>
        /// Ensures the update document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        private object EnsureUpdateDocument(object document)
        {
            var descriptor = _configuration.SerializationFactory.GetObjectDescriptor(typeof(T));

            var foundOp = descriptor.GetMongoPropertyNames(document)
                .Any(name => name.IndexOf('$') == 0);

            if(foundOp == false)
            {
                //wrap document in a $set.
                return new Document().Add("$set", document);
            }

            return document;
        }
    }
}
