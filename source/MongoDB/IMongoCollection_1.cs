using System;
using System.Collections.Generic;

namespace MongoDB
{
    /// <summary>
    ///   A collection is a storage unit for a group of <see cref = "Document" />s.  The documents do not all have to 
    ///   contain the same schema but for efficiency they should all be similar.
    /// </summary>
    /// <remarks>
    ///   Safemode checks the database for any errors that may have occurred during 
    ///   the insert such as a duplicate key constraint violation.
    /// </remarks>
    public interface IMongoCollection<T>
        where T : class
    {
        /// <summary>
        ///   Gets the database.
        /// </summary>
        /// <value>The database.</value>
        IMongoDatabase Database { get; }

        /// <summary>
        ///   Name of the collection.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///   String value of the database name.
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        ///   Full name of the collection databasename . collectionname
        /// </summary>
        string FullName { get; }

        /// <summary>
        ///   Metadata about the collection such as indexes.
        /// </summary>
        CollectionMetadata Metadata { get; }

        /// <summary>
        /// Finds and returns the first document in a selector query.
        /// </summary>
        /// <param name="javascriptWhere">The where.</param>
        /// <returns>
        /// A <see cref="Document"/> from the collection.
        /// </returns>
        T FindOne(string javascriptWhere);

        /// <summary>
        /// Finds and returns the first document in a selector query.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns>
        /// A <see cref="Document"/> from the collection.
        /// </returns>
        T FindOne(object selector);

        /// <summary>
        ///   Returns a cursor that contains all of the documents in the collection.
        /// </summary>
        /// <remarks>
        ///   Cursors load documents from the database in batches instead of all at once.
        /// </remarks>
        ICursor<T> FindAll();

        /// <summary>
        /// Uses the $where operator to query the collection.  The value of the where is Javascript that will
        /// produce a true for the documents that match the criteria.
        /// </summary>
        /// <param name="javascriptWhere">Javascript</param>
        /// <returns></returns>
        ICursor<T> Find(string javascriptWhere);

        /// <summary>
        /// Queries the collection using the query selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns>A <see cref="ICursor"/></returns>
        ICursor<T> Find(object selector);

        /// <summary>
        /// Queries the collection using the specification and only returns a subset of fields.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>A <see cref="ICursor"/></returns>
        ICursor<T> Find(object selector, object fields);

        /// <summary>
        /// Deprecated.  Use the fluent interface on the cursor to specify a limit and skip value.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        [Obsolete("Use the fluent interface on ICursor for specifying limit and skip Find.Skip(x).Limit(y)")]
        ICursor<T> Find(object selector, int limit, int skip);

        /// <summary>
        /// Queries the collection using the specification and only returns a subset of fields
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        [Obsolete("Use the fluent interface on ICursor for specifying limit and skip Find.Skip(x).Limit(y)")]
        ICursor<T> Find(object selector, int limit, int skip, object fields);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>A <see cref="Document"/></returns>
        T FindAndModify(object document, object selector);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="sort"><see cref="Document"/> containing the names of columns to sort on with the values being the</param>
        /// <returns>A <see cref="Document"/></returns>
        /// <see cref="IndexOrder"/>
        T FindAndModify(object document, object selector, object sort);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="returnNew">if set to <c>true</c> [return new].</param>
        /// <returns>A <see cref="Document"/></returns>
        T FindAndModify(object document, object selector, bool returnNew);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="sort"><see cref="Document"/> containing the names of columns to sort on with the values being the
        /// <see cref="IndexOrder"/></param>
        /// <param name="returnNew">if set to <c>true</c> [return new].</param>
        /// <param name="fields">The fields.</param>
        /// <param name="upsert">if set to <c>true</c> [upsert].</param>
        /// <returns>A <see cref="Document"/></returns>
        T FindAndModify(object document, object selector, object sort, bool returnNew,object fields,bool upsert);

        /// <summary>
        ///   Entrypoint into executing a map/reduce query against the collection.
        /// </summary>
        /// <returns></returns>
        MapReduce MapReduce();

        ///<summary>
        ///  Count all items in the collection.
        ///</summary>
        long Count();

        /// <summary>
        ///   Count all items in a collection that match the query selector.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <returns></returns>
        /// <remarks>
        ///   It will return 0 if the collection doesn't exist yet.
        /// </remarks>
        long Count(object selector);

        /// <summary>
        ///   Inserts the Document into the collection.
        /// </summary>
        void Insert(object document, bool safemode);

        /// <summary>
        ///   Inserts the specified doc.
        /// </summary>
        /// <param name = "document">The doc.</param>
        void Insert(object document);

        /// <summary>
        ///   Bulk inserts the specified documents into the database.
        /// </summary>
        /// <remarks>
        ///   See the safemode description in the class description
        /// </remarks>
        void Insert<TElement>(IEnumerable<TElement> documents, bool safemode);

        /// <summary>
        ///   Bulk inserts the specified documents into the database.
        /// </summary>
        /// <param name = "documents">The documents.</param>
        void Insert<TElement>(IEnumerable<TElement> documents);

        /// <summary>
        ///   Deletes documents from the collection according to the selector.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   An empty document will match all documents in the collection and effectively truncate it.
        ///   See the safemode description in the class description
        /// </remarks>
        [Obsolete("Use Remove instead")]
        void Delete(object selector, bool safemode);

        /// <summary>
        ///   Remove documents from the collection according to the selector.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   An empty document will match all documents in the collection and effectively truncate it.
        ///   See the safemode description in the class description
        /// </remarks>
        void Remove(object selector, bool safemode);

        /// <summary>
        ///   Deletes documents from the collection according to the selector.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <remarks>
        ///   An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        [Obsolete("Use Remove instead")]
        void Delete(object selector);

        /// <summary>
        /// Remove documents from the collection according to the selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <remarks>
        /// An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        void Remove(object selector);

        /// <summary>
        ///   Inserts or updates a document in the database.  If the document does not contain an _id one will be
        ///   generated and an upsert sent.  Otherwise the document matching the _id of the document will be updated.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   See the safemode description in the class description
        /// </remarks>
        [Obsolete("Use Save instead")]
        void Update(object document, bool safemode);

        /// <summary>
        ///   Inserts or updates a document in the database.  If the document does not contain an _id one will be
        ///   generated and an upsert sent.  Otherwise the document matching the _id of the document will be updated.
        /// </summary>
        /// <param name = "document">The document.</param>
        [Obsolete("Use Save instead")]
        void Update(object document);

        /// <summary>
        ///   Updates the specified document with the current document.  In order to only do a partial update use a
        ///   document containing modifier operations ($set, $unset, $inc, etc.)
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   See the safemode description in the class description
        /// </remarks>
        void Update(object document, object selector, bool safemode);

        /// <summary>
        ///   Updates the specified document with the current document.  In order to only do a partial update use a 
        ///   document containing modifier operations ($set, $unset, $inc, etc.)
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void Update(object document, object selector);

        /// <summary>
        ///   Updates the specified document with the current document.  In order to only do a partial update use a
        ///   document containing modifier operations ($set, $unset, $inc, etc.)
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "flags">The flags.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   See the safemode description in the class description
        /// </remarks>
        void Update(object document, object selector, UpdateFlags flags, bool safemode);

        /// <summary>
        ///   Updates the specified document with the current document.  In order to only do a partial update use a 
        ///   document containing modifier operations ($set, $unset, $inc, etc.)
        /// </summary>
        /// <param name = "document">The <see cref = "Document" /> to update with</param>
        /// <param name = "selector">The query selector to find the document to update.</param>
        /// <param name = "flags"><see cref = "UpdateFlags" /></param>
        void Update(object document, object selector, UpdateFlags flags);

        /// <summary>
        ///   Runs a multiple update query against the database.  It will wrap any
        ///   doc with $set if the passed in doc doesn't contain any '$' modifier ops.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void UpdateAll(object document, object selector);

        /// <summary>
        ///   Runs a multiple update query against the database.  It will wrap any
        ///   doc with $set if the passed in doc doesn't contain any '$' modifier ops.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   See the safemode description in the class description
        /// </remarks>
        void UpdateAll(object document, object selector, bool safemode);

        /// <summary>
        ///   Inserts or updates a document in the database.  If the document does not contain an _id one will be
        ///   generated and an upsert sent.  Otherwise the document matching the _id of the document will be updated.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <remarks>
        ///   The document will contain the _id that is saved to the database.
        /// </remarks>
        void Save(object document);

        /// <summary>
        ///   Saves a document to the database using an upsert.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Save(object document, bool safemode);

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
        T FindAndModify(object document, object spec, object sort, bool returnNew);
    }
}