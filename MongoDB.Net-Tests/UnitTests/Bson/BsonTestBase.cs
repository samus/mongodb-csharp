using System;
using System.IO;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.UnitTests.Bson
{
    public abstract class BsonTestBase
    {
        protected string Serialize(Document document)
        {
            return Serialize(document, new BsonWriterSettings());
        }

        protected string Serialize(Document document, BsonWriterSettings settings)
        {
            using(var mem = new MemoryStream())
            {
                var writer = new BsonWriter(mem, settings);
                writer.WriteObject(document);
                writer.Flush();
                return Convert.ToBase64String(mem.ToArray());
            }
        }

        protected Document Deserialize(string base64){
            return Deserialize(base64, new BsonReaderSettings());
        }

        protected Document Deserialize(string base64, BsonReaderSettings settings)
        {
            using(var mem = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var reader = new BsonReader(mem, settings);
                return (Document)reader.ReadObject();
            }
        }
    }
}