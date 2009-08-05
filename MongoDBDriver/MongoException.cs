using System;
using System.Runtime.Serialization;
using System.Text;

namespace MongoDB.Driver
{
    [Serializable]
    public class MongoException : Exception, ISerializable
    {
        public MongoException()
        {
            // Add implementation.
        }
        public MongoException(string message): base(message)
        {
            // Add implementation.
        }
        public MongoException(string message, Exception inner): base(message,inner)
        {
            // Add implementation.
        }

        // This constructor is needed for serialization.
        protected MongoException(SerializationInfo info, StreamingContext context):base(info,context)
        {
            // Add implementation.
        }

    }
}
