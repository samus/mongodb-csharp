using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Raised when an update action causes a unique constraint violation in an index.
    /// </summary>
    /// <remarks>
    /// It is only another class because Mongo makes a distinction and it may be helpful.
    /// </remarks>
    public class MongoDuplicateKeyUpdateException : MongoDuplicateKeyException
    {
        public MongoDuplicateKeyUpdateException(string message, Document error)
            :base(message,error){}
        public MongoDuplicateKeyUpdateException(string message, Document error, Exception e):base(message, error,e){} 
    }
}