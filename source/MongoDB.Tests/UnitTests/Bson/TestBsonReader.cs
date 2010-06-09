using System;
using System.IO;
using System.Text;
using MongoDB.Bson;
using MongoDB.Util;
using NUnit.Framework;

namespace MongoDB.UnitTests.Bson
{
    [TestFixture]
    public class TestBsonReader : BsonTestBase
    {
        private char pound = '\u00a3';
        private char euro = '\u20ac';

        private string WriteAndReadString(string val){
            var buf = Encoding.UTF8.GetBytes(val + '\0');

            var ms = new MemoryStream(buf);
            var reader = new BsonReader(ms, new BsonDocumentBuilder());
            return reader.ReadString();
        }

        private string WriteAndReadLenString(string val){
            var ms = new MemoryStream();
            var bs = new BsonWriter(ms, new BsonDocumentDescriptor());
            var w = new BinaryWriter(ms);
            var byteCount = bs.CalculateSize(val, false);
            w.Write(byteCount);
            bs.Write(val, false);
            ms.Seek(0, SeekOrigin.Begin);
            var reader = new BsonReader(ms, new BsonDocumentBuilder());
            return reader.ReadLengthString();
        }

        [Test]
        public void TestReadDocWithDocs(){
            //            Document doc = new Document().Append("a", new Document().Append("b", new Document().Append("c",new Document())));
            //            Console.WriteLine(ConvertDocToHex(doc));
            var buf = HexToBytes("1D000000036100150000000362000D0000000363000500000000000000");
            var ms = new MemoryStream(buf);
            var reader = new BsonReader(ms, new BsonDocumentBuilder());

            var doc = (Document)reader.ReadObject();
            Assert.IsNotNull(doc, "Document was null");
            Assert.AreEqual(buf.Length, reader.Position);
            Assert.IsTrue(doc.Contains("a"));
        }

        [Test]
        public void TestReadEmptyDocument(){
            var buf = HexToBytes("0500000000");
            var ms = new MemoryStream(buf);
            var reader = new BsonReader(ms, new BsonDocumentBuilder());

            var doc = (Document)reader.ReadObject();

            Assert.IsNotNull(doc);
        }

        [Test]
        public void TestReadLenString(){
            const string expected = "test";
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadLenStringLong(){
            var sb = new StringBuilder();
            sb.Append('t', 150);
            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadLenStringShortTripleByte(){
            var sb = new StringBuilder();
            //sb.Append('1',127); //first char of euro at the end of the boundry.
            //sb.Append(euro, 5);
            //sb.Append('1',128);
            sb.Append(euro);

            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadLenStringTripleByteCharBufferBoundry0(){
            var sb = new StringBuilder();
            sb.Append('1', 127); //first char of euro at the end of the boundry.
            sb.Append(euro, 5);
            sb.Append('1', 128);
            sb.Append(euro);

            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadLenStringTripleByteCharBufferBoundry1(){
            var sb = new StringBuilder();
            sb.Append('1', 126);
            sb.Append(euro, 5); //middle char of euro at the end of the boundry.
            sb.Append('1', 128);
            sb.Append(euro);

            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadLenStringTripleByteCharBufferBoundry2(){
            var sb = new StringBuilder();
            sb.Append('1', 125);
            sb.Append(euro, 5); //last char of the eruo at the end of the boundry.
            sb.Append('1', 128);
            sb.Append(euro);

            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadLenStringTripleByteCharOne(){
            var sb = new StringBuilder();
            sb.Append(euro, 1); //Just one triple byte char in the string.

            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadLenStringValue(){
            const string expected = "test";

            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }

        [Test]
        public void TestReadMultiElementDocument(){
            var buf = HexToBytes("2D000000075F6964004A753AD8FAC16EA58B290351016100000000000000F03F02620005000000746573740000");
            var ms = new MemoryStream(buf);
            var reader = new BsonReader(ms, new BsonDocumentBuilder());

            var doc = (Document)reader.ReadObject();

            Assert.IsNotNull(doc, "Document was null");
            Assert.IsTrue(doc.Contains("_id"));
            Assert.IsTrue(doc.Contains("a"));
            Assert.IsTrue(doc.Contains("b"));
            Assert.AreEqual("4a753ad8fac16ea58b290351", (doc["_id"]).ToString());
            Assert.AreEqual(1, Convert.ToInt32(doc["a"]));
            Assert.AreEqual("test", doc["b"]);
        }

        [Test]
        public void TestReadSimpleDocument(){
            var buf = HexToBytes("1400000002746573740005000000746573740000");
            var ms = new MemoryStream(buf);
            var reader = new BsonReader(ms, new BsonDocumentBuilder());

            var doc = reader.Read();

            Assert.IsNotNull(doc, "Document was null");
            Assert.IsTrue(doc.Contains("test"));
            Assert.AreEqual("test", doc["test"]);
        }

        [Test]
        public void TestReadString(){
            var buf = HexToBytes("7465737400");
            var ms = new MemoryStream(buf);
            var reader = new BsonReader(ms, new BsonDocumentBuilder());

            var s = reader.ReadString();
            Assert.AreEqual("test", s);
            Assert.AreEqual(4, Encoding.UTF8.GetByteCount(s));
        }
        
        [Test]
        public void TestReadBigDocument(){
            MemoryStream ms = new MemoryStream();
            var writer = new BsonWriter(ms, new BsonDocumentDescriptor());
            
            Document expected = new Document();
            expected.Append("str", "test")
                .Append("int", 45)
                .Append("long", (long)46)
                .Append("num", 4.5)
                .Append("date",DateTime.Today)
                .Append("_id", new OidGenerator().Generate())
                .Append("code", new Code("return 1;"))
                .Append("subdoc", new Document().Append("a",1).Append("b",2))                
                .Append("array", new String[]{"a","b","c","d"})
                .Append("codewscope", new CodeWScope("return 2;", new Document().Append("c",1)))
                .Append("binary", new Binary(new byte[]{0,1,2,3}))
                .Append("regex", new MongoRegex("[A-Z]"))
                .Append("minkey", MongoMinKey.Value)
                .Append("maxkey", MongoMaxKey.Value)
                .Append("symbol", new MongoSymbol("symbol"))
            ;
            writer.WriteObject(expected);
            writer.Flush();
            ms.Seek(0,SeekOrigin.Begin);           
            
            BsonReader reader = new BsonReader(ms, new BsonDocumentBuilder());
            Document doc = reader.Read();
            
            Assert.IsNotNull(doc);
        }

        [Test]
        public void TestReadStringBreakDblByteCharOverBuffer(){
            var sb = new StringBuilder();
            sb.Append('1', 127);
            sb.Append(pound); //will break the pound symbol over the buffer boundry.
            //sb.Append("1");

            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }

        [Test]
        public void TestReadStringDblByteCharOnEndOfBufferBoundry(){
            var sb = new StringBuilder();
            sb.Append(pound, 66); //puts a pound symbol at the end of the buffer boundry but not broken.
            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }

        [Test]
        public void TestReadStringLong(){
            var sb = new StringBuilder();
            sb.Append('t', 256);
            var expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }

        [Test]
        public void TestReadStringTripleByteCharBufferBoundry(){
            var sb = new StringBuilder();
            sb.Append("12");
            sb.Append(euro, 66); //will break the euro symbol over the buffer boundry.

            var expected = sb.ToString();

            Assert.AreEqual(expected, WriteAndReadString(expected));
        }

        [Test]
        public void TestReadStringWithUkPound(){
            const string expected = "1234£56";
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }

        [Test]
        public void TestReadUtcTimeByDefault(){
            var document = Deserialize("EwAAAAl0aW1lAADJU+klAQAAAA==");

            var dateTime = new DateTime(2010, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            
            Assert.AreEqual(dateTime, document["time"]);
        }

        [Test]
        public void TestReadUtcTimeToLocalTime(){
            var settings = new BsonReaderSettings {ReadLocalTime = true};

            var document = Deserialize("EwAAAAl0aW1lAADJU+klAQAAAA==", settings);

            var localtzoffset =TimeZoneInfo.Local.BaseUtcOffset.Hours - 1; //gmt offset the local date was saved in along with the local offset.

            var dateTime = new DateTime(2010, 1, 1, 11, 0, 0, DateTimeKind.Local).AddHours(localtzoffset);
            Assert.AreEqual(dateTime, document["time"]);
        }
    }
}