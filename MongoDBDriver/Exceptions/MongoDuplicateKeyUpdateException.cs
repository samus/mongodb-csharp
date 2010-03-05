using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Raised when an update action causes a unique constraint violation in an index.
    /// </summary>
    /// <remarks>
    /// It is only another class because Mongo makes a distinction and it may be helpful.
    /// </remarks>
    public class MongoDuplicateKeyUpdateException : MongoDuplicateKeyException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDuplicateKeyUpdateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        public MongoDuplicateKeyUpdateException(string message, Document error)
            :base(message,error){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDuplicateKeyUpdateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        /// <param name="e">The e.</param>
        public MongoDuplicateKeyUpdateException(string message, Document error, Exception e):base(message, error,e){} 
    }
}