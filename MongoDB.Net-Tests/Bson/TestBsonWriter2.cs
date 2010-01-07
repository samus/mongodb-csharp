using System;
using System.IO;

using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonWriter2
    {
        [Test]
        public void TestCalculateSizeOfEmptyDoc(){
            Document doc = new Document();
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            
            Assert.AreEqual(5,writer.CalculateSize(doc));
        }
        
        [Test]
        public void TestCalculateSizeOfSimpleDoc(){
            Document doc = new Document();
            doc.Append("a","a");
            doc.Append("b",1);
            
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            //BsonDocument bdoc = BsonConvert.From(doc);
            
            Assert.AreEqual(21,writer.CalculateSize(doc));
        }
        
        [Test]
        public void TestCalculateSizeOfComplexDoc(){
            Document doc = new Document();
            doc.Append("a","a");
            doc.Append("b",1);
            Document sub = new Document().Append("c_1",1).Append("c_2",DateTime.Now);
            doc.Append("c",sub);
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            
            Assert.AreEqual(51,writer.CalculateSize(doc));            
        }
        
        [Test]
        public void TestWriteString(){           
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            string expected = "54-65-73-74-73-2E-69-6E-73-65-72-74-73-00";
            writer.WriteString("Tests.inserts");
            
            string hexdump = BitConverter.ToString(ms.ToArray());
            
            Assert.AreEqual(expected, hexdump);
        }
        
        [Test]
        public void TestWriteDocument(){
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            string expected = "1400000002746573740005000000746573740000";
            Document doc = new Document().Append("test", "test");
            
            writer.WriteDocument(doc);
            
            string hexdump = BitConverter.ToString(ms.ToArray());
            hexdump = hexdump.Replace("-","");
            
            Assert.AreEqual(expected, hexdump);
        }
        
        [Test]
        public void TestWriteArrayDoc(){
            String expected = "2000000002300002000000610002310002000000620002320002000000630000";
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            
            String[] str = new String[]{"a","b","c"};
            writer.WriteValue(BsonDataType.Array,str);
            
            string hexdump = BitConverter.ToString(ms.ToArray());
            hexdump = hexdump.Replace("-","");
            Assert.AreEqual(expected, hexdump);
        }
    }
}
