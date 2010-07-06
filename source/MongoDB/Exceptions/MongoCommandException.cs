using System;
using System.Runtime.Serialization;

namespace MongoDB
{
    /// <summary>
    ///   Raised when a command returns a failure message.
    /// </summary>
    [Serializable]
    public class MongoCommandException : MongoException
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoCommandException" /> class.
        /// </summary>
        /// <param name = "message">The message.</param>
        /// <param name = "error">The error.</param>
        /// <param name = "command">The command.</param>
        public MongoCommandException(string message, Document error, Document command)
            : base(message, null)
        {
            Error = error;
            Command = command;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoCommandException" /> class.
        /// </summary>
        /// <param name = "message">The message.</param>
        /// <param name = "error">The error.</param>
        /// <param name = "command">The command.</param>
        /// <param name = "e">The e.</param>
        public MongoCommandException(string message, Document error, Document command, Exception e)
            : base(message, e)
        {
            Error = error;
            Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCommandException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        public MongoCommandException(SerializationInfo info, StreamingContext context) : base(info,context)
        {
        }

        /// <summary>
        ///   Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public Document Error { get; private set; }

        /// <summary>
        ///   Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public Document Command { get; private set; }
    }
}