using System;

namespace MongoDB.GridFS
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoGridFSException : Exception
    {
        private string filename;
        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename
        {
            get { return filename; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoGridFSException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="inner">The inner.</param>
        public MongoGridFSException(string message, string filename, Exception inner)
            : base(message, inner)
        {
            this.filename = filename;
        }
    }    
}
