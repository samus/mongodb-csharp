using System;
using System.IO;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Serialization
{
    public abstract class SerializationTestBase
    {
        protected string Serialize(Document document)
        {
            using(var mem = new MemoryStream())
            {
                var writer = new BsonWriter(mem, new DocumentDescriptor());
                writer.WriteObject(document);
                writer.Flush();
                return Convert.ToBase64String(mem.ToArray());
            }
        }

        protected string Serialize(object instance)
        {
            using(var mem = new MemoryStream())
            {
                var writer = new BsonWriter(mem, new BsonReflectionDescriptor());
                writer.WriteObject(instance);
                writer.Flush();
                return Convert.ToBase64String(mem.ToArray());
            }
        }

        protected T Deserialize<T>(string base64)
        {
            using(var mem = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var reader = new BsonReader(mem, new BsonReflectionBuilder(typeof(T)));
                return (T)reader.ReadObject();
            }
        }

        protected Document DeserializeDocument(string base64)
        {
            using(var mem = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var reader = new BsonReader(mem);
                return (Document)reader.ReadObject();
            }
        }
    }
}