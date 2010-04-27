using System;
using System.IO;
using System.Text;
using MongoDB.Driver.Tests.Bson;
using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonWriter : BsonTestBase
    {
        private char euro = '\u20ac';

        private string WriteStringAndGetHex(string val){
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            writer.Write(val, false);
            return BitConverter.ToString(ms.ToArray());
        }

        [Test]
        public void TestCalculateSizeOfComplexDoc(){
            var doc = new Document();
            doc.Add("a", "a");
            doc.Add("b", 1);
            var sub = new Document().Add("c_1", 1).Add("c_2", DateTime.Now);
            doc.Add("c", sub);
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());

            Assert.AreEqual(51, writer.CalculateSizeObject(doc));
        }

        [Test]
        public void TestCalculateSizeOfEmptyDoc(){
            var doc = new Document();
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());

            Assert.AreEqual(5, writer.CalculateSizeObject(doc));
        }

        [Test]
        public void TestCalculateSizeOfSimpleDoc(){
            var doc = new Document();
            doc.Add("a", "a");
            doc.Add("b", 1);

            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            //BsonDocument bdoc = BsonConvert.From(doc);

            Assert.AreEqual(21, writer.CalculateSizeObject(doc));
        }

        [Test]
        public void TestNullsDontThrowExceptions(){
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            var doc = new Document().Add("n", null);
            try
            {
                writer.WriteObject(doc);
            }
            catch(NullReferenceException)
            {
                Assert.Fail("Null Reference Exception was thrown on trying to serialize a null value");
            }
        }

        [Test]
        public void TestWriteArrayDoc(){
            const string expected = "2000000002300002000000610002310002000000620002320002000000630000";
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());

            var str = new[] {"a", "b", "c"};
            writer.WriteValue(BsonDataType.Array, str);

            var hexdump = BitConverter.ToString(ms.ToArray());
            hexdump = hexdump.Replace("-", "");
            Assert.AreEqual(expected, hexdump);
        }

        [Test]
        public void TestWriteDocument(){
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            const string expected = "1400000002746573740005000000746573740000";
            var doc = new Document().Add("test", "test");

            writer.WriteObject(doc);

            var hexdump = BitConverter.ToString(ms.ToArray());
            hexdump = hexdump.Replace("-", "");

            Assert.AreEqual(expected, hexdump);
        }

        [Test]
        public void TestWriteMultibyteString(){
            var val = new StringBuilder().Append(euro, 3).ToString();
            var expected = BitConverter.ToString(Encoding.UTF8.GetBytes(val + '\0'));
            Assert.AreEqual(expected, WriteStringAndGetHex(val));
        }

        [Test]
        public void TestWriteMultibyteStringLong(){
            var val = new StringBuilder().Append("ww").Append(euro, 180).ToString();
            var expected = BitConverter.ToString(Encoding.UTF8.GetBytes(val + '\0'));
            Assert.AreEqual(expected, WriteStringAndGetHex(val));
        }

        [Test]
        public void TestWriteString(){
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            const string expected = "54-65-73-74-73-2E-69-6E-73-65-72-74-73-00";
            writer.Write("Tests.inserts", false);

            var hexdump = BitConverter.ToString(ms.ToArray());

            Assert.AreEqual(expected, hexdump);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), UserMessage = "Shouldn't be able to write large document")]
        public void TestWritingTooLargeDocument(){
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            var b = new Binary(new byte[BsonInfo.MaxDocumentSize]);
            var big = new Document().Add("x", b);

            writer.WriteObject(big);
        }

        [Test]
        public void TestWriteUtcTimeByDefault(){
            var dateTime = new DateTime(2010, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            
            var base64 = Serialize(new Document("time", dateTime));

            Assert.AreEqual("EwAAAAl0aW1lAADJU+klAQAAAA==",base64);
        }

        [Test]
        public void TestLocalDateTimeIsWrittenAsUtcTime()
        {
            var dateTime = new DateTime(2010, 1, 1, 10, 0, 0, DateTimeKind.Local);

            var base64 = Serialize(new Document("time", dateTime));

            Assert.AreEqual("EwAAAAl0aW1lAIDaHOklAQAAAA==", base64);
        }
        
        [Test]
        public void TestWriteSingle(){
            string expected = "000000E0FFFFEF47";
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            Single val = Single.MaxValue;
            
            writer.WriteValue(BsonDataType.Number, val);
            
            string hexdump = BitConverter.ToString(ms.ToArray());
            hexdump = hexdump.Replace("-","");
            Assert.AreEqual(expected, hexdump);
            
            
        }
        
        [Test]
        public void TestWriteSymbol(){
            string expected = "0700000073796D626F6C00";
                   
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            MongoSymbol val = "symbol";
            Assert.IsTrue(String.IsInterned(val) != null);
            writer.WriteValue(BsonDataType.Symbol, val);
            string hexdump = BitConverter.ToString(ms.ToArray()).Replace("-","");
            
            Assert.AreEqual(expected, hexdump);
        }        
    }
}