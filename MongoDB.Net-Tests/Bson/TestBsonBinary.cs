using System;
using System.IO;
using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]  
	public class TestBsonBinary
	{
        [Test]
        public void TestBinary()
        {
            
            byte[] data = File.ReadAllBytes(@"test-data\tests.binary.txt");
            BsonBinary binaryIn = new BsonBinary(new Binary(data));
            MemoryStream stream = new MemoryStream();
            BsonWriter bsonWriter = new BsonWriter(stream);
            binaryIn.Write(bsonWriter);

            stream.Position = 0;
            BsonReader reader = new BsonReader(stream);
            BsonBinary binaryOut = new BsonBinary();
            int size = reader.ReadInt32();
            binaryOut.Subtype = reader.ReadByte();
            binaryOut.Val = reader.ReadBytes(size);
            Assert.AreEqual(binaryIn.Val, binaryOut.Val);
            Assert.AreEqual(binaryIn.Subtype, binaryOut.Subtype);
            Assert.AreEqual(data.Length, binaryOut.Size);
        }
	}
}
