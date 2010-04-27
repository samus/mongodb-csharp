using System;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
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
    }
}