using System;
using System.Collections.Generic;

namespace MongoDB {
    /// <summary>
    /// 
    /// </summary>
    public interface ICursor : IDisposable
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
        ICursor Spec(Document spec);
        
        /// <summary>
        /// Limits the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        ICursor Limit(int limit);
        
        /// <summary>
        /// Skips the specified skip.
        /// </summary>
        /// <param name="skip">The skip.</param>
        /// <returns></returns>
        ICursor Skip(int skip);
        
        /// <summary>
        /// Fieldses the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        ICursor Fields(Document fields);
        
        /// <summary>
        /// Optionses the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        ICursor Options(QueryOptions options);
        
        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        ICursor Sort(string field);
        
        /// <summary>
        /// Sorts the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        ICursor Sort(string field, IndexOrder order);
        
        /// <summary>
        /// Sorts the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        ICursor Sort(Document fields);
        
        /// <summary>
        /// Hints the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        ICursor Hint(Document index);
        
        /// <summary>
        /// Snapshots this instance.
        /// </summary>
        /// <returns></returns>
        ICursor Snapshot();
        
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
        IEnumerable<Document> Documents { get; }
    }
}
