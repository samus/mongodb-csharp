using System;
using System.IO;

using NUnit.Framework;

using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    [TestFixture]
    public class TestInsertMessage
    {
        [Test]
        public void TestWrite(){
            MemoryStream ms = new MemoryStream();
            
            InsertMessage im = new InsertMessage();
            im.FullCollectionName ="Tests.inserts";
            Document doc = new Document();
            Document[] docs = new Document[]{doc};
            doc.Append("a","a");
            doc.Append("b",1);
            im.Documents = docs;
            
            Assert.AreEqual(16,im.Header.MessageLength);
            
            BsonWriter2 bwriter = new BsonWriter2(ms);
            
            Assert.AreEqual(21,bwriter.CalculateSize(doc));
            
            im.Write(ms);
            
            Byte[] bytes = ms.ToArray();
            String hexdump = BitConverter.ToString(bytes);
            System.Console.WriteLine(hexdump);
            
            MemoryStream ms2 = new MemoryStream();
            BsonWriter b = new BsonWriter(ms2);
            b.Write(im.Header.MessageLength);
            b.Write(im.Header.RequestId);
            b.Write(im.Header.ResponseTo);
            b.Write((int)im.Header.OpCode);
            b.Write((int)0);
            b.Write(im.FullCollectionName);
            BsonDocument bdoc = BsonConvert.From(doc);
            bdoc.Write(b);
            
            b.Flush();
            String hexdump2 = BitConverter.ToString(ms2.ToArray());
            System.Console.WriteLine(hexdump2);
            Assert.AreEqual(hexdump2,hexdump);
            Assert.AreEqual(bytes.Length,im.Header.MessageLength);
        }
    }
}
