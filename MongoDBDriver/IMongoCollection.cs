using System;
using System.Collections.Generic;

namespace MongoDB.Driver
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMongoCollection{
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        string DatabaseName { get; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        string FullName { get; }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        CollectionMetaData MetaData { get; }

        /// <summary>
        /// Finds the one.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        Document FindOne (Document spec);

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <returns></returns>
        ICursor FindAll ();

        /// <summary>
        /// Finds the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        ICursor Find (String @where);

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        ICursor Find (Document spec);

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        ICursor Find (Document spec, int limit, int skip);

        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        ICursor Find (Document spec, int limit, int skip, Document fields);

        /// <summary>
        /// Maps the reduce.
        /// </summary>
        /// <returns></returns>
        MapReduce MapReduce ();

        /// <summary>
        /// Maps the reduce builder.
        /// </summary>
        /// <returns></returns>
        MapReduceBuilder MapReduceBuilder ();

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        long Count ();

        /// <summary>
        /// Counts the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        long Count (Document spec);

        /// <summary>
        /// Inserts the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        void Insert (Document doc);

        /// <summary>
        /// Inserts the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void Insert (Document doc, bool safemode);

        /// <summary>
        /// Inserts the specified docs.
        /// </summary>
        /// <param name="docs">The docs.</param>
        void Insert (IEnumerable<Document> docs);

        /// <summary>
        /// Inserts the specified docs.
        /// </summary>
        /// <param name="docs">The docs.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void Insert (IEnumerable<Document> docs, bool safemode);

        /// <summary>
        /// Deletes the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        void Delete (Document selector);

        /// <summary>
        /// Deletes the specified selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void Delete (Document selector, bool safemode);

        /// <summary>
        /// Updates the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        void Update (Document doc);

        /// <summary>
        /// Updates the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="selector">The selector.</param>
        void Update (Document doc, Document selector);

        /// <summary>
        /// Updates the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="flags">The flags.</param>
        void Update (Document doc, Document selector, UpdateFlags flags);

        /// <summary>
        /// Updates the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void Update (Document doc, bool safemode);

        /// <summary>
        /// Updates the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void Update (Document doc, Document selector, bool safemode);

        /// <summary>
        /// Updates the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void Update (Document doc, Document selector, UpdateFlags flags, bool safemode);

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="selector">The selector.</param>
        void UpdateAll (Document doc, Document selector);

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="safemode">if set to <c>true</c> [safemode].</param>
        void UpdateAll (Document doc, Document selector, bool safemode);

        /// <summary>
        /// Saves the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        void Save (Document doc);
    }
}