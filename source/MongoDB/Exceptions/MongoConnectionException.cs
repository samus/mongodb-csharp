using System;
using System.Runtime.Serialization;
using MongoDB.Connections;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MongoConnectionException : MongoException
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>The end point.</value>
        public MongoServerEndPoint EndPoint { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="endPoint">The end point.</param>
        public MongoConnectionException(string message, string connectionString, MongoServerEndPoint endPoint)
            : this(message,connectionString,endPoint,null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="innerException">The inner exception.</param>
        public MongoConnectionException(string message, string connectionString, MongoServerEndPoint endPoint, Exception innerException)
            : base(message, innerException){
            EndPoint = endPoint;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        internal MongoConnectionException(string message, Connection connection)
            :this(message,connection,null){}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="innerException">The inner exception.</param>
        internal MongoConnectionException(string message, Connection connection, Exception innerException)
            :base(message,innerException){
            if(connection == null)
                throw new ArgumentNullException("connection");
            ConnectionString = connection.ConnectionString;
            EndPoint = connection.EndPoint;
        }  
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConnectionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        public MongoConnectionException(SerializationInfo info, StreamingContext context)
         : base(info, context)
        {
        }     
    }
}