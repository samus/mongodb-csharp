using System;
using System.Collections.Generic;

namespace MongoDB.Driver{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICursor<T> : IDisposable
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        long Id { get; }

        /// <summary>
        /// Specs the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        ICursor<T> Spec(object spec);

        /// <summary>
        /// Limits the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        ICursor<T> Limit(int limit);

        /// <summary>
        /// Skips the specified skip.
        /// </summary>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        ICursor<T> Skip(int skip);

        /// <summary>
        /// Fieldses the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        ICursor<T> Fields(object fields);

        /// <summary>
        /// Optionses the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        ICursor<T> Options(QueryOptions options);

        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        ICursor<T> Sort(string field);

        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        ICursor<T> Sort(string field, IndexOrder order);

        /// <summary>
        /// Sorts the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        ICursor<T> Sort(object fields);

        /// <summary>
        /// Hints the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        ICursor<T> Hint(object index);

        /// <summary>
        /// Snapshots this instance.
        /// </summary>
        /// <returns></returns>
        ICursor<T> Snapshot();

        /// <summary>
        /// Explains this instance.
        /// </summary>
        /// <returns></returns>
        Document Explain();

        /// <summary>
        /// Gets a value indicating whether this instance is modifiable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modifiable; otherwise, <c>false</c>.
        /// </value>
        bool IsModifiable { get; }

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <value>The documents.</value>
        IEnumerable<T> Documents { get; }
    }
}
