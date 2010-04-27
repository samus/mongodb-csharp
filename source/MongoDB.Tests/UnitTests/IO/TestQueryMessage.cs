using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Protocol;
using NUnit.Framework;

namespace MongoDB.UnitTests.IO
{
    [TestFixture]
    public class TestQueryMessage
    {
        [Test]
        public void TestAllBytesWritten()
        {
            Document query = new Document();
            query.Add("col1", 1);
            
            var msg = new QueryMessage(new BsonDocumentDescriptor(),query,"TestDB.TestCol");
            MemoryStream buffer = new MemoryStream();
            msg.Write(buffer);
            
            Byte[] output = buffer.ToArray();
            String hexdump = BitConverter.ToString(output);
            //Console.WriteLine("Dump: " + hexdump);

            Assert.IsTrue(output.Length > 0);
            Assert.AreEqual("3A-00-00-00-00-00-00-00-00-00-00-00-D4-07-00-00-00-00-00-00-54-65-73-74-44-42-2E-54-65-73-74-43-6F-6C-00-00-00-00-00-00-00-00-00-0F-00-00-00-10-63-6F-6C-31-00-01-00-00-00-00", hexdump);
            
        }
        
        [Test]
        public void TestWriteMessageTwice(){
            string expectedHex = "3A-00-00-00-00-00-00-00-00-00-00-00-D4-07-00-00-00-00-00-00-54-65-73-74-44-42-2E-54-65-73-74-43-6F-6C-00-00-00-00-00-00-00-00-00-0F-00-00-00-10-63-6F-6C-31-00-01-00-00-00-00";
            Document query = new Document();
            query.Add("col1", 1);
            
            var msg = new QueryMessage(new BsonDocumentDescriptor(),query,"TestDB.TestCol");
            MemoryStream buffer = new MemoryStream();
            msg.Write(buffer);
            
            Byte[] output = buffer.ToArray();
            String hexdump = BitConverter.ToString(output);
            
            MemoryStream buffer2 = new MemoryStream();
            msg.Write(buffer2);
            
            Byte[] output2 = buffer.ToArray();
            String hexdump2 = BitConverter.ToString(output2);
            
            Assert.AreEqual(expectedHex,hexdump);
            Assert.AreEqual(hexdump,hexdump2);
                        
        }
    }
}