using System;
using System.IO;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization;
using MongoDB.Driver.Tests.Bson;

namespace MongoDB.Driver.Serialization
{
    public abstract class SerializationTestBase : BsonTestBase
    {
        protected string Serialize(object instance){
            return Serialize(instance, instance.GetType());
        }

        protected string Serialize(object instance,Type rootType)
        {
            using(var mem = new MemoryStream())
            {
                var writer = new BsonWriter(mem, new BsonReflectionDescriptor(SerializationFactory.Default, rootType));
                writer.WriteObject(instance);
                writer.Flush();
                return Convert.ToBase64String(mem.ToArray());
            }
        }

        protected T Deserialize<T>(string base64)
        {
            using(var mem = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var reader = new BsonReader(mem, new BsonReflectionBuilder(SerializationFactory.Default,typeof(T)));
                return (T)reader.ReadObject();
            }
        }
    }
}