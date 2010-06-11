using System;

namespace MongoDB
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnmappedMemberException : MongoException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnmappedMemberException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnmappedMemberException(string message) : base(message) { }
    }
}
