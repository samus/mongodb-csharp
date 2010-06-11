using System;
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
    }
}
