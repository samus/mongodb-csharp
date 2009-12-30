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
    }
}
