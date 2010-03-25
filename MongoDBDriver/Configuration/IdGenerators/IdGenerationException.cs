using System;

namespace MongoDB.Driver.Configuration.IdGenerators
{
    [global::System.Serializable]
    public class IdGenerationException : Exception
    {
        public IdGenerationException(string message) : base(message) { }
        protected IdGenerationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
