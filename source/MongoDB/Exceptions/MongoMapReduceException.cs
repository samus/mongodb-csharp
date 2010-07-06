using System;
using System.Runtime.Serialization;
using MongoDB.Results;

namespace MongoDB 
{
    /// <summary>
    /// Raised when a map reduce call fails. 
    /// </summary>
    [Serializable]
    public class MongoMapReduceException : MongoCommandException
    {
        /// <summary>
        /// Gets or sets the map reduce result.
        /// </summary>
        /// <value>The map reduce result.</value>
        public MapReduceResult MapReduceResult { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoMapReduceException"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public MongoMapReduceException(MongoCommandException exception)
            :base(exception.Message,exception.Error, exception.Command) {
            MapReduceResult = new MapReduceResult(exception.Error);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoMapReduceException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        public MongoMapReduceException(SerializationInfo info, StreamingContext context)
         : base(info, context)
        {
        }
    }
}
