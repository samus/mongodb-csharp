using System;
using System.Runtime.Serialization;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MongoOperationException : MongoException
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public Document Error { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        public MongoOperationException(string message, Document error):this(message, error,null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        /// <param name="e">The e.</param>
        public MongoOperationException(string message, Document error, Exception e):base(message,e){
            this.Error = error;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoOperationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        public MongoOperationException(SerializationInfo info, StreamingContext context)
         : base(info, context)
        {
        }        
    }
}