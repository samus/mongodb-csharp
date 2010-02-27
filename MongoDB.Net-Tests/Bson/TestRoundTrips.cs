using System;
using System.IO;

using MongoDB.Driver.Bson;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestRoundTrips
    {

        [Test]
        public void TestDBRef(){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms);

            Document source = new Document();
            source.Append("x",1).Append("ref",new DBRef("refs","ref1"));

            writer.Write(source);
            writer.Flush();
            ms.Seek(0,SeekOrigin.Begin);

            BsonReader reader = new BsonReader(ms);
            Document copy = reader.Read();

            Assert.IsTrue(copy.Contains("ref"));
            Assert.IsTrue(copy["ref"].GetType() == typeof(DBRef));

            DBRef sref = (DBRef)source["ref"];
            DBRef cref = (DBRef)copy["ref"];

            Assert.AreEqual(sref.Id, cref.Id);

        }

        [Test]
        public void TestDateLocal(){
            DateTime now = DateTime.Now;
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms);

            Document source = new Document();
            source.Append("d",now);

            writer.Write(source);
            writer.Flush();
            ms.Seek(0,SeekOrigin.Begin);
            
            BsonReader reader = new BsonReader(ms);
            Document copy = reader.Read();
            
            DateTime then = (DateTime)copy["d"];           
            then = then.ToLocalTime();
            
            Assert.AreEqual(now.Hour,then.Hour, "Date did not round trip right.");

        }
        
        [Test]
        public void TestDateUTC(){
            DateTime now = DateTime.UtcNow;
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms);

            Document source = new Document();
            source.Append("d",now);

            writer.Write(source);
            writer.Flush();
            ms.Seek(0,SeekOrigin.Begin);
            
            BsonReader reader = new BsonReader(ms);
            Document copy = reader.Read();
            
            DateTime then = (DateTime)copy["d"];           
            
            Assert.AreEqual(now.Hour,then.Hour, "Date did not round trip right.");

        }        
    }
}
