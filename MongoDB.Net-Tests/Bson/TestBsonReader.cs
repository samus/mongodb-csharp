using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonReader
    {
        char pound = '\u00a3';
        char euro = '\u20ac';
        
        [Test]
        public void TestReadString(){
            byte[] buf = HexToBytes("7465737400");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader reader = new BsonReader(ms);
            
            String s = reader.ReadString();
            Assert.AreEqual("test",s);
            Assert.AreEqual(4,Encoding.UTF8.GetByteCount(s));
        }
        
        [Test]
        public void TestReadStringLong(){
            StringBuilder sb = new StringBuilder();
            sb.Append('t',256);
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }
        
        [Test]
        public void TestReadStringWithUKPound(){
            string expected = "1234£56";
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }
        
        [Test]
        public void TestReadStringBreakDblByteCharOverBuffer(){
            StringBuilder sb = new StringBuilder();
            sb.Append('1',127);
            sb.Append(pound); //will break the pound symbol over the buffer boundry.
            //sb.Append("1");
            
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }
        
        [Test]
        public void TestReadStringDblByteCharOnEndOfBufferBoundry(){
            StringBuilder sb = new StringBuilder();
            sb.Append(pound, 66); //puts a pound symbol at the end of the buffer boundry but not broken.
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadString(expected));
        }

        [Test]
        public void TestReadStringTripleByteCharBufferBoundry(){
            StringBuilder sb = new StringBuilder();
            sb.Append("12");
            sb.Append(euro, 66); //will break the euro symbol over the buffer boundry.
            
            string expected = sb.ToString();

            Assert.AreEqual(expected, WriteAndReadString(expected));
        }
        
        private string WriteAndReadString(string val){
            byte[] buf = Encoding.UTF8.GetBytes(val + '\0');
            
            MemoryStream ms = new MemoryStream(buf);
            BsonReader reader = new BsonReader(ms);
            return reader.ReadString();            
        }
        
        [Test]
        public void TestReadLenStringValue(){
            string expected = "test";
            
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }
        
        [Test]
        public void TestReadLenStringShortTripleByte(){
            StringBuilder sb = new StringBuilder();
            //sb.Append('1',127); //first char of euro at the end of the boundry.
            //sb.Append(euro, 5);
            //sb.Append('1',128);
            sb.Append(euro);
            
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }
        
        [Test]
        public void TestReadLenStringTripleByteCharBufferBoundry0(){
            StringBuilder sb = new StringBuilder();
            sb.Append('1',127); //first char of euro at the end of the boundry.
            sb.Append(euro, 5);
            sb.Append('1',128);
            sb.Append(euro);
            
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }
        
        [Test]
        public void TestReadLenStringTripleByteCharBufferBoundry1(){
            StringBuilder sb = new StringBuilder();
            sb.Append('1',126);
            sb.Append(euro, 5); //middle char of euro at the end of the boundry.
            sb.Append('1',128);
            sb.Append(euro);
            
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }
        
        [Test]
        public void TestReadLenStringTripleByteCharOne(){
            StringBuilder sb = new StringBuilder();
            sb.Append(euro, 1); //Just one triple byte char in the string.
            
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }        
        
        [Test]
        public void TestReadLenStringTripleByteCharBufferBoundry2(){
            StringBuilder sb = new StringBuilder();
            sb.Append('1', 125);
            sb.Append(euro, 5); //last char of the eruo at the end of the boundry.
            sb.Append('1',128);
            sb.Append(euro);
            
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));            
        }

        [Test]
        public void TestReadLenString(){
            string expected = "test";
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }
        
        [Test]
        public void TestReadLenStringLong(){
            StringBuilder sb = new StringBuilder();
            sb.Append('t',150);
            string expected = sb.ToString();
            Assert.AreEqual(expected, WriteAndReadLenString(expected));
        }
        
        private string WriteAndReadLenString(string val){
            MemoryStream ms = new MemoryStream();
            BsonWriter bs = new BsonWriter(ms);            
            BinaryWriter w = new BinaryWriter(ms);
            int byteCount = bs.CalculateSize(val,false);           
            w.Write(byteCount);
            bs.Write(val,false);
            ms.Seek(0,SeekOrigin.Begin);
            BsonReader reader = new BsonReader(ms);
            return reader.ReadLengthString();
        }
        
        
        [Test]
        public void TestReadEmptyDocument(){
            byte[] buf = HexToBytes("0500000000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader reader = new BsonReader(ms);
            
            Document doc = (Document)reader.ReadObject();
            
            Assert.IsNotNull(doc);
        }
        
        [Test]
        public void TestReadSimpleDocument(){
            byte[] buf = HexToBytes("1400000002746573740005000000746573740000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader reader = new BsonReader(ms);
            
            Document doc = reader.Read();
            
            Assert.IsNotNull(doc, "Document was null");
            Assert.IsTrue(doc.Contains("test"));
            Assert.AreEqual("test", (String)doc["test"]);
        }
        
        [Test]
        public void TestReadMultiElementDocument(){
            byte[] buf = HexToBytes("2D000000075F6964004A753AD8FAC16EA58B290351016100000000000000F03F02620005000000746573740000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader reader = new BsonReader(ms);
            
            Document doc = (Document)reader.ReadObject();
            
            Assert.IsNotNull(doc, "Document was null");
            Assert.IsTrue(doc.Contains("_id"));
            Assert.IsTrue(doc.Contains("a"));
            Assert.IsTrue(doc.Contains("b"));
            Assert.AreEqual("\"4a753ad8fac16ea58b290351\"", ((Oid)doc["_id"]).ToString());
            Assert.AreEqual(1, Convert.ToInt32(doc["a"]));
            Assert.AreEqual("test", (String)doc["b"]);
        }
                
        [Test]
        public void TestReadDocWithDocs(){
//            Document doc = new Document().Append("a", new Document().Append("b", new Document().Append("c",new Document())));
//            Console.WriteLine(ConvertDocToHex(doc));
            byte[] buf = HexToBytes("1D000000036100150000000362000D0000000363000500000000000000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader reader = new BsonReader(ms);
            
            Document doc = (Document)reader.ReadObject();
            Assert.IsNotNull(doc, "Document was null");
            Assert.AreEqual(buf.Length, reader.Position);
            Assert.IsTrue(doc.Contains("a"));
            
        }
        
        [Test]
        public void TestReadBigDocument(){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms);
            
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
            ;
            writer.WriteObject(expected);
            writer.Flush();
            ms.Seek(0,SeekOrigin.Begin);           
            
            BsonReader reader = new BsonReader(ms);
            Document doc = reader.Read();
            
            Assert.IsNotNull(doc);
        }
        
        private String ConvertDocToHex(Document doc){
            MemoryStream ms = new MemoryStream();
            BsonWriter writer = new BsonWriter(ms);
            
            writer.WriteObject(doc);
            return BitConverter.ToString(ms.ToArray()).Replace("-","");
                        
        }
        
        private byte[] HexToBytes(string hex){
            //TODO externalize somewhere.
            if(hex.Length % 2 == 1){
                System.Console.WriteLine("uneven number of hex pairs.");
                hex = "0" + hex;
            }           
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2){
                try{
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }
                catch{
                    //failed to convert these 2 chars, they may contain illegal charracters
                    bytes[i / 2] = 0;
                }
            }
            return bytes;
        }        
        
    }
}
