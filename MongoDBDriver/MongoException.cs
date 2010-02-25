using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Base class for all Mongo Exceptions
    /// </summary>
    public class MongoException : Exception
    {
        public MongoException(string message, Exception inner):base(message,inner){}
    }
}