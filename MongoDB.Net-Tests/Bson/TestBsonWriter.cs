using System;
using System.IO;
using System.Text;
using MongoDB.Driver.Tests.Bson;
using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonWriter : BsonTestBase
    {
        char euro = '\u20ac';
        [Test]
        public void TestCalculateSizeOfEmptyDoc(){
            Document doc = new Document();
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            
            Assert.AreEqual(5,writer.CalculateSizeObject(doc));
        }
        
        [Test]
        public void TestCalculateSizeOfSimpleDoc(){
            Document doc = new Document();
            doc.Add("a","a");
            doc.Add("b",1);
            
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            //BsonDocument bdoc = BsonConvert.From(doc);
            
            Assert.AreEqual(21,writer.CalculateSizeObject(doc));
        }
        
        [Test]
        public void TestCalculateSizeOfComplexDoc(){
            Document doc = new Document();
            doc.Add("a","a");
            doc.Add("b",1);
            Document sub = new Document().Add("c_1",1).Add("c_2",DateTime.Now);
            doc.Add("c",sub);
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            
            Assert.AreEqual(51,writer.CalculateSizeObject(doc));            
        }
        
        [Test]
        public void TestWriteString(){           
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            string expected = "54-65-73-74-73-2E-69-6E-73-65-72-74-73-00";
            writer.Write("Tests.inserts",false);
            
            string hexdump = BitConverter.ToString(ms.ToArray());
            
            Assert.AreEqual(expected, hexdump);
        }
        
        [Test]
        public void TestWriteMultibyteString(){
            
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            
            string val = new StringBuilder().Append(euro,3).ToString();
            string expected = BitConverter.ToString(Encoding.UTF8.GetBytes(val + '\0'));
            Assert.AreEqual(expected,WriteStringAndGetHex(val));
        }
        
        [Test]
        public void TestWriteMultibyteStringLong(){
            
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());

            string val = new StringBuilder().Append("ww").Append(euro, 180).ToString();
            string expected = BitConverter.ToString(Encoding.UTF8.GetBytes(val + '\0'));
            Assert.AreEqual(expected,WriteStringAndGetHex(val));
        }
        
        private string WriteStringAndGetHex(string val){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            writer.Write(val,false);
            return BitConverter.ToString(ms.ToArray());
        }
        
        [Test]
        public void TestWriteDocument(){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            string expected = "1400000002746573740005000000746573740000";
            Document doc = new Document().Add("test", "test");
            
            writer.WriteObject(doc);
            
            string hexdump = BitConverter.ToString(ms.ToArray());
            hexdump = hexdump.Replace("-","");
            
            Assert.AreEqual(expected, hexdump);
        }
        
        [Test]
        public void TestWriteArrayDoc(){
            String expected = "2000000002300002000000610002310002000000620002320002000000630000";
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            
            String[] str = new String[]{"a","b","c"};
            writer.WriteValue(BsonDataType.Array,str);
            
            string hexdump = BitConverter.ToString(ms.ToArray());
            hexdump = hexdump.Replace("-","");
            Assert.AreEqual(expected, hexdump);
        }

        [Test]
        public void TestNullsDontThrowExceptions(){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            Document doc = new Document().Add("n", null);
            try{
                writer.WriteObject(doc);
            }catch(NullReferenceException){
                Assert.Fail("Null Reference Exception was thrown on trying to serialize a null value");
            }
        }
        
        [Test]
        public void TestWritingTooLargeDocument(){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            Binary b = new Binary(new byte[BsonInfo.MaxDocumentSize]);
            Document big = new Document().Add("x", b);
            bool thrown = false;
            try{
                writer.WriteObject(big);    
            }catch(ArgumentException){
                thrown = true;
            }catch(Exception e){
                Assert.Fail("Wrong Exception thrown " + e.GetType().Name);
            }
            
            Assert.IsTrue(thrown, "Shouldn't be able to write large document");
        }

        [Test]
        public void TestWriteDateTimeFromOtherTimeZone(){
            var europeStd = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            var referenceTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2010, 1, 1, 10, 0, 0, DateTimeKind.Utc), europeStd);

            var bson = Serialize(new Document("Value", referenceTime));

            var document = Deserialize(bson);

            Assert.AreEqual(referenceTime, document["Value"]);
        }
    }
}
