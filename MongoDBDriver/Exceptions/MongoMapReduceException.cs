namespace MongoDB.Driver 
{
    /// <summary>
    /// Raised when a map reduce call fails. 
    /// </summary>
    public class MongoMapReduceException : MongoCommandException
    {
        /// <summary>
        /// Gets or sets the map reduce result.
        /// </summary>
        /// <value>The map reduce result.</value>
        public MapReduce.MapReduceResult MapReduceResult { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoMapReduceException"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="mapReduce">The map reduce.</param>
        public MongoMapReduceException(MongoCommandException exception, MapReduce mapReduce)
            :base(exception.Message,exception.Error, exception.Command) {
            MapReduceResult = new MapReduce.MapReduceResult(exception.Error);
        }
    }
}
