using System;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class IdGenerationException : MongoException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdGenerationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public IdGenerationException(string message) : base(message) { }
    }
}
