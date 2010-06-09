using System;
using System.IO;
using MongoDB.Bson;

namespace MongoDB.UnitTests.Bson
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


        protected byte[] HexToBytes(string hex)
        {
            //TODO externalize somewhere.
            if (hex.Length % 2 == 1)
            {
                Console.WriteLine("uneven number of hex pairs.");
                hex = "0" + hex;
            }
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                try
                {
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }
                catch
                {
                    //failed to convert these 2 chars, they may contain illegal charracters
                    bytes[i / 2] = 0;
                }
            return bytes;
        }
    }
}