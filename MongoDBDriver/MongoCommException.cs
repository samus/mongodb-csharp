using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoCommException : MongoException
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCommException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        public MongoCommException(string message, Connection connection):this(message,connection,null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCommException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="inner">The inner.</param>
        public MongoCommException(string message, Connection connection, Exception inner):base(message,inner){
            ConnectionString = connection.ConnectionString;
        }       
    }
}