using System;
using System.IO;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Tests.Serialization
{
    public abstract class SerializationTestBase
    {
        protected string Serialize(Document document)
        {
            using(var mem = new MemoryStream())
            {
                var writer = new BsonWriter(mem, new BsonDocumentDescriptor());
                writer.WriteObject(document);
                writer.Flush();
                return Convert.ToBase64String(mem.ToArray());
            }
        }

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

        protected Document DeserializeDocument(string base64)
        {
            using(var mem = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var reader = new BsonReader(mem,new BsonDocumentBuilder());
                return (Document)reader.ReadObject();
            }
        }
    }
}