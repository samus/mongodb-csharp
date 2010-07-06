using System;
using System.Runtime.Serialization;

namespace MongoDB
{
    /// <summary>
    /// Raised when an action causes a unique constraint violation in an index. 
    /// </summary>
    [Serializable]
    public class MongoDuplicateKeyException : MongoOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        public MongoDuplicateKeyException(string message, Document error):base(message, error,null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        /// <param name="e">The e.</param>
        public MongoDuplicateKeyException(string message, Document error, Exception e):base(message, error,e){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        public MongoDuplicateKeyException(SerializationInfo info, StreamingContext context)
         : base(info, context)
        {
        }   
    }
}