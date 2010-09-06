using System;
using System.Collections.Generic;

namespace MongoDB
{
    /// <summary>
    /// </summary>
    [Obsolete("Uses IMongoCollection<Document> instead. This class will be possible not included in future releases.")]
    public interface IMongoCollection
    {
        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        ///   Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        string DatabaseName { get; }

        /// <summary>
        ///   Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        string FullName { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        IMongoDatabase Database { get; }

        /// <summary>
        ///   Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        CollectionMetadata Metadata { get; }

        /// <summary>
        /// Finds the one.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        Document FindOne(Document selector);

        /// <summary>
        ///   Finds all.
        /// </summary>
        /// <returns></returns>
        ICursor FindAll();

        /// <summary>
        ///   Finds the specified where.
        /// </summary>
        /// <param name = "where">The where.</param>
        /// <returns></returns>
        ICursor Find(String @where);

        /// <summary>
        /// Finds the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        ICursor Find(Document selector);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <param name = "limit">The limit.</param>
        /// <param name = "skip">The skip.</param>
        /// <returns></returns>
        ICursor Find(Document spec, int limit, int skip);

        /// <summary>
        ///   Finds the specified spec.
        /// </summary>
        /// <param name = "spec">The spec.</param>
        /// <param name = "limit">The limit.</param>
        /// <param name = "skip">The skip.</param>
        /// <param name = "fields">The fields.</param>
        /// <returns></returns>
        ICursor Find(Document spec, int limit, int skip, Document fields);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>A <see cref="Document"/></returns>
        Document FindAndModify(Document document, Document selector);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="sort"><see cref="Document"/> containing the names of columns to sort on with the values being the</param>
        /// <returns>A <see cref="Document"/></returns>
        /// <see cref="IndexOrder"/>
        Document FindAndModify(Document document, Document selector, Document sort);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="returnNew">if set to <c>true</c> [return new].</param>
        /// <returns>A <see cref="Document"/></returns>
        Document FindAndModify(Document document, Document selector, bool returnNew);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="sort"><see cref="Document"/> containing the names of columns to sort on with the values being the
        /// <see cref="IndexOrder"/></param>
        /// <param name="returnNew">if set to <c>true</c> [return new].</param>
        /// <returns>A <see cref="Document"/></returns>
        Document FindAndModify(Document document, Document selector, Document sort, bool returnNew);
        
        /// <summary>
        ///   Maps the reduce.
        /// </summary>
        /// <returns></returns>
        MapReduce MapReduce();

        /// <summary>
        ///   Counts this instance.
        /// </summary>
        /// <returns></returns>
        long Count();

        /// <summary>
        /// Counts the specified spec.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        long Count(Document selector);

        /// <summary>
        ///   Inserts the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        void Insert(Document document);

        /// <summary>
        ///   Inserts the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Insert(Document document, bool safemode);

        /// <summary>
        ///   Inserts the specified docs.
        /// </summary>
        /// <param name = "documents">The docs.</param>
        void Insert(IEnumerable<Document> documents);

        /// <summary>
        ///   Inserts the specified docs.
        /// </summary>
        /// <param name = "documents">The docs.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Insert(IEnumerable<Document> documents, bool safemode);

        /// <summary>
        ///   Deletes the specified selector.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        [Obsolete("Use Remove instead")]
        void Delete(Document selector);

        /// <summary>
        /// Removes the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Remove(Document selector);

        /// <summary>
        ///   Deletes the specified selector.
        /// </summary>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        [Obsolete("Use Remove instead")]
        void Delete(Document selector, bool safemode);

        /// <summary>
        /// Removes the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void Remove(Document selector, bool safemode);

        /// <summary>
        ///   Updates the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        [Obsolete("Use Save instead")]
        void Update(Document document);

        /// <summary>
        ///   Updates the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void Update(Document document, Document selector);

        /// <summary>
        ///   Updates the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "flags">The flags.</param>
        void Update(Document document, Document selector, UpdateFlags flags);

        /// <summary>
        ///   Updates the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        [Obsolete("Use Save instead")]
        void Update(Document document, bool safemode);

        /// <summary>
        ///   Updates the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(Document document, Document selector, bool safemode);

        /// <summary>
        ///   Updates the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "flags">The flags.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Update(Document document, Document selector, UpdateFlags flags, bool safemode);

        /// <summary>
        ///   Updates all.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        void UpdateAll(Document document, Document selector);

        /// <summary>
        ///   Updates all.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void UpdateAll(Document document, Document selector, bool safemode);

        /// <summary>
        ///   Saves the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        void Save(Document document);

        /// <summary>
        ///   Saves the specified doc.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "safemode">if set to <c>true</c> [safemode].</param>
        void Save(Document document, bool safemode);

        /// <summary>
        /// Executes a query and atomically applies a modifier operation to the first document returning the original document
        /// by default.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="spec"><see cref="Document"/> to find the document.</param>
        /// <param name="sort"><see cref="Document"/> containing the names of columns to sort on with the values being the
        /// <see cref="IndexOrder"/></param>
        /// <param name="returnNew">if set to <c>true</c> [return new].</param>
        /// <param name="fields">The fields.</param>
        /// <param name="upsert">if set to <c>true</c> [upsert].</param>
        /// <returns>A <see cref="Document"/></returns>
        Document FindAndModify(Document document, Document spec, Document sort, bool returnNew, Document fields, bool upsert);
    }
}