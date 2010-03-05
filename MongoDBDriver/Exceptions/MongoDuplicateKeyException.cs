using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Raised when an action causes a unique constraint violation in an index. 
    /// </summary>
    public class MongoDuplicateKeyException : MongoOperationException
    {
        public MongoDuplicateKeyException(string message, Document error):base(message, error,null){}
        public MongoDuplicateKeyException(string message, Document error, Exception e):base(message, error,e){}
    }
}