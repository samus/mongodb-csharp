using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Base class for all Mongo Exceptions
    /// </summary>
    public class MongoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MongoException(string message, Exception inner):base(message,inner){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MongoException(string message):base(message){}
    }
}