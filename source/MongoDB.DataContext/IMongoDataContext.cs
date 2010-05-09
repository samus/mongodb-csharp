using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Configuration;

namespace MongoDB.DataContext
{
    public interface IMongoDataContext : IDisposable
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        IMongoConfiguration Configuration { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        IMongoDatabase Database { get; }

        /// <summary>
        /// Begins a queryable find.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> Find<T>() where T : class;

        /// <summary>
        /// Finds all the documents matching the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Finds all the documents matching the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IEnumerable<T> Find<T>(object predicate) where T : class;

        /// <summary>
        /// Removes the specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        void Remove<T>(T document) where T : class;

        /// <summary>
        /// Removes all the entities matching the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        void Remove<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Removes all the entities matching the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        void Remove<T>(object predicate) where T : class;

        /// <summary>
        /// Saves the specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        void Save<T>(T document) where T : class;

        /// <summary>
        /// Updates all the documents that match the predicate with the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="predicate">The predicate.</param>
        void Update<T>(object values, object predicate) where T : class;

        /// <summary>
        /// Updates all the documents that match the predicate with the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="predicate">The predicate.</param>
        void Update<T>(object values, Expression<Func<T, bool>> predicate) where T : class;
    }
}
