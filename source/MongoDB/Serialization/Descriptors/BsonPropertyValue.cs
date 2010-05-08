using System;

namespace MongoDB.Serialization.Descriptors
{
    internal class BsonPropertyValue
    {
        public Type Type { get; private set; }

        public object Value { get; private set; }

        public BsonPropertyValue(Type type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}