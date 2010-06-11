using System;

namespace MongoDB
{
    /// <summary>
    /// Base class for all Mongo Exceptions
    /// </summary>
    [Serializable]
    public class MongoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner.</param>
        public MongoException(string message, Exception innerException):base(message,innerException){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MongoException(string message):base(message){}
    }
}