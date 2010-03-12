using System;
using System.Collections.Generic;

namespace MongoDB.Driver
{
    public interface IMongoCollection<T> where T : class
    {
        /// <summary>
        ///   Gets the database.
        /// </summary>
        /// <value>The database.</value>
        Database Database { get; }

        /// <summary>
        ///   Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        ///   Gets or sets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        string DatabaseName { get; }

        /// <summary>
        ///   Gets the full name including database name.
        /// </summary>
        /// <value>The full name.</value>
        string FullName { get; }

        /// <summary>
        ///   Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        CollectionMetaData MetaData { get; }

        /// <summary>
        ///   Finds and returns the first document in a query.
        /// </summary>
        /// <param name = "spec">A <see cref = "Document" /> representing the query.</param>
        /// <returns>
        ///   A <see cref = "Document" /> from the collection.
        /// </returns>
        T FindOne(Document spec);

        /// <summary>
        ///   Finds and returns the first document in a query.
        /// </summary>
        /// <param name = "spec">A <see cref = "Document" /> representing the query.</param>
        /// <returns>
        ///   A <see cref = "Document" /> from the collection.
        /// </returns>
        T FindOne(object spec);

        /// <summary>
        ///   Finds all.
        /// </summary>
        /// <returns></returns>
        ICursor<T> FindAll();

        /// <summary>
        ///   Finds the specified where.
        /// </summary>
        /// <param name = "where">The where.</param>
        /// <returns></returns>
        ICursor<T> Find(String where);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <returns></returns>
        ICursor<T> Find(Document spec);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <returns></returns>
        ICursor<T> Find(object spec);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <param name = "limit">The limit.</param>
        /// <param name = "skip">The skip.</param>
        /// <returns></returns>
        ICursor<T> Find(Document spec, int limit, int skip);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <param name = "limit">The limit.</param>
        /// <param name = "skip">The skip.</param>
        /// <returns></returns>
        ICursor<T> Find(object spec, int limit, int skip);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <param name = "limit">The limit.</param>
        /// <param name = "skip">The skip.</param>
        /// <param name = "fields">The fields.</param>
        /// <returns></returns>
        ICursor<T> Find(Document spec, int limit, int skip, Document fields);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <param name = "limit">The limit.</param>
        /// <param name = "skip">The skip.</param>
        /// <param name = "fields">The fields.</param>
        /// <returns></returns>
        ICursor<T> Find(object spec, int limit, int skip, object fields);

        /// <summary>
        ///   Entrypoint into executing a map/reduce query against the collection.
        /// </summary>
        /// <returns></returns>
        MapReduce MapReduce();

        /// <summary>
        ///   Maps the reduce builder.
        /// </summary>
        /// <returns></returns>
        MapReduceBuilder MapReduceBuilder();

        ///<summary>
        ///  Count all items in the collection.
        ///</summary>
        long Count();

        /// <summary>
        ///   Count all items in a collection that match the query spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <returns></returns>
        /// <remarks>
        ///   It will return 0 if the collection doesn't exist yet.
        /// </remarks>
        long Count(Document spec);

        /// <summary>
        ///   Count all items in a collection that match the query spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <returns></returns>
        /// <remarks>
        ///   It will return 0 if the collection doesn't exist yet.
        /// </remarks>
        long Count(object spec);

        /// <summary>
        ///   Inserts the Document into the collection.
        /// </summary>
        void Insert(Document document, bool safemode);

        /// <summary>
        ///   Inserts the Document into the collection.
        /// </summary>
        void Insert(object document, bool safemode);

        /// <summary>
        ///   Inserts the specified doc.
        /// </summary>
        /// <param name = "document">The doc.</param>
        void Insert(Document document);

        /// <summary>
        ///   Inserts the specified doc.
        /// </summary>
        /// <param name = "document">The doc.</param>
        void Insert(object document);

        /// <summary>
        ///   Inserts the specified documents.
        /// </summary>
        /// <param name = "documents">The documents.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Insert(IEnumerable<Document> documents, bool safemode);

        /// <summary>
        ///   Inserts the specified documents.
        /// </summary>
        /// <param name = "documents">The documents.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Insert<TElement>(IEnumerable<TElement> documents, bool safemode);

        /// <summary>
        ///   Inserts the specified documents.
        /// </summary>
        /// <param name = "documents">The documents.</param>
        void Insert(IEnumerable<Document> documents);

        /// <summary>
        ///   Inserts the specified documents.
        /// </summary>
        /// <param name = "documents">The documents.</param>
        void Insert<TElement>(IEnumerable<TElement> documents);

        /// <summary>
        ///   Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        void Delete(Document selector, bool safemode);

        /// <summary>
        ///   Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        /// <remarks>
        ///   An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        void Delete(object selector, bool safemode);

        /// <summary>
        ///   Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <remarks>
        ///   An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        void Delete(Document selector);

        /// <summary>
        ///   Deletes documents from the collection according to the spec.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <remarks>
        ///   An empty document will match all documents in the collection and effectively truncate it.
        /// </remarks>
        void Delete(object selector);

        /// <summary>
        ///   Updates the specified document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(Document document, bool safemode);

        /// <summary>
        ///   Updates the specified document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(object document, bool safemode);

        /// <summary>
        ///   Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <remarks>
        ///   _id will be used in the document to create a selector.  If it isn't in
        ///   the document then it is assumed that the document is new and an upsert is sent to the database
        ///   instead.
        /// </remarks>
        void Update(Document document);

        /// <summary>
        ///   Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <remarks>
        ///   _id will be used in the document to create a selector.  If it isn't in
        ///   the document then it is assumed that the document is new and an upsert is sent to the database
        ///   instead.
        /// </remarks>
        void Update(object document);

        /// <summary>
        ///   Updates the specified document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(Document document, Document selector, bool safemode);

        /// <summary>
        ///   Updates the specified document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(object document, object selector, bool safemode);

        /// <summary>
        ///   Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void Update(Document document, Document selector);

        /// <summary>
        ///   Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void Update(object document, object selector);

        /// <summary>
        ///   Updates the specified document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "flags">The flags.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(Document document, Document selector, UpdateFlags flags, bool safemode);

        /// <summary>
        ///   Updates the specified document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "flags">The flags.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(object document, object selector, UpdateFlags flags, bool safemode);

        /// <summary>
        ///   Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name = "document">The <see cref = "Document" /> to update with</param>
        /// <param name = "selector">The query spec to find the document to update.</param>
        /// <param name = "flags"><see cref = "UpdateFlags" /></param>
        void Update(Document document, Document selector, UpdateFlags flags);

        /// <summary>
        ///   Updates a document with the data in doc as found by the selector.
        /// </summary>
        /// <param name = "document">The <see cref = "Document" /> to update with</param>
        /// <param name = "selector">The query spec to find the document to update.</param>
        /// <param name = "flags"><see cref = "UpdateFlags" /></param>
        void Update(object document, object selector, UpdateFlags flags);

        /// <summary>
        ///   Runs a multiple update query against the database.  It will wrap any
        ///   doc with $set if the passed in doc doesn't contain any '$' ops.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void UpdateAll(Document document, Document selector);

        /// <summary>
        ///   Runs a multiple update query against the database.  It will wrap any
        ///   doc with $set if the passed in doc doesn't contain any '$' ops.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void UpdateAll(object document, object selector);

        /// <summary>
        ///   Updates all.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void UpdateAll(Document document, Document selector, bool safemode);

        /// <summary>
        ///   Updates all.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void UpdateAll(object document, object selector, bool safemode);

        /// <summary>
        ///   Saves a document to the database using an upsert.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <remarks>
        ///   The document will contain the _id that is saved to the database.  This is really just an alias
        ///   to Update(Document) to maintain consistency between drivers.
        /// </remarks>
        void Save(Document document);

        /// <summary>
        ///   Saves a document to the database using an upsert.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <remarks>
        ///   The document will contain the _id that is saved to the database.  This is really just an alias
        ///   to Update(Document) to maintain consistency between drivers.
        /// </remarks>
        void Save(object document);
    }
}