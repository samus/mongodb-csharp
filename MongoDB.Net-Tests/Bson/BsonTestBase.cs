using System;
using System.IO;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Tests.Bson
{
    public abstract class BsonTestBase
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

        protected Document Deserialize(string base64)
        {
            using(var mem = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var reader = new BsonReader(mem, new BsonDocumentBuilder());
                return (Document)reader.ReadObject();
            }
        }
    }
}