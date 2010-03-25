using System;

namespace MongoDB.Driver.Serialization
{
    [Serializable]
    public class UnmappedMemberException : Exception
    {
        public UnmappedMemberException(string message) : base(message) { }
        protected UnmappedMemberException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
