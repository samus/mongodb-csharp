using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonReader2
    {
        [Test]
        public void TestReadString(){
            byte[] buf = HexToBytes("7465737400");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
            String s = reader.ReadString();
            Assert.AreEqual("test",s);
            Assert.AreEqual(4,Encoding.UTF8.GetByteCount(s));
        }
        
        [Test]
        public void TestReadLongerString(){
            byte[] buf = HexToBytes("7465737474657374746573747465737474657374746573747465737474657374746573747465737400");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
            String s = reader.ReadString();
            Assert.AreEqual("testtesttesttesttesttesttesttesttesttest",s);
        }
        
        [Test]
        public void TestReadStringWithUKPound(){
            byte[] buf = HexToBytes("31323334C2A3353600");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
            String s = reader.ReadString();
            Assert.AreEqual("1234£56",s);
            Assert.AreEqual(8,Encoding.UTF8.GetByteCount(s));            
        }
        
        [Test]
        public void TestReadStringValue(){
            byte[] buf = HexToBytes("050000007465737400");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
			String str = reader.ReadLenString();
            int read = Encoding.UTF8.GetByteCount(str) + 5;
            Assert.AreEqual(buf.Length, read);
            Assert.AreEqual("test", (String)str);
        }
                
        [Test]
        public void TestReadStringElement(){
            byte[] buf = HexToBytes("027465737400050000007465737400");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            Document doc = new Document();
            
            int read = reader.ReadElement(doc);
            //Assert.AreEqual(buf.Length,read);
            Assert.IsTrue(doc.Contains("test"));
            Assert.AreEqual("test",(String)doc["test"]);
			Assert.AreEqual(buf.Length,read);
        }
        
        [Test]
        public void TestReadEmptyDocument(){
            byte[] buf = HexToBytes("0500000000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
            Document doc = null;
            int size = reader.ReadDocument(ref doc);
            
        }
        
        [Test]
        public void TestReadSimpleDocument(){
            byte[] buf = HexToBytes("1400000002746573740005000000746573740000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
            Document doc = reader.Read();
            
            Assert.IsNotNull(doc, "Document was null");
            Assert.IsTrue(doc.Contains("test"));
            Assert.AreEqual("test", (String)doc["test"]);
        }
        
        [Test]
        public void TestReadMultiElementDocument(){
            byte[] buf = HexToBytes("2D000000075F6964004A753AD8FAC16EA58B290351016100000000000000F03F02620005000000746573740000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
            Document doc = new Document();
            reader.ReadDocument(ref doc);
            
            Assert.IsNotNull(doc, "Document was null");
            Assert.IsTrue(doc.Contains("_id"));
            Assert.IsTrue(doc.Contains("a"));
            Assert.IsTrue(doc.Contains("b"));
            Assert.AreEqual("ObjectId(\"4a753ad8fac16ea58b290351\")", ((Oid)doc["_id"]).ToString());
            Assert.AreEqual(1, Convert.ToInt32(doc["a"]));
            Assert.AreEqual("test", (String)doc["b"]);
        }
                
        [Test]
        public void TestReadDocWithDocs(){
//            Document doc = new Document().Append("a", new Document().Append("b", new Document().Append("c",new Document())));
//            Console.WriteLine(ConvertDocToHex(doc));
            byte[] buf = HexToBytes("1D000000036100150000000362000D0000000363000500000000000000");
            MemoryStream ms = new MemoryStream(buf);
            BsonReader2 reader = new BsonReader2(ms);
            
            Document doc = new Document();
            int read = reader.ReadDocument(ref doc);
            Assert.IsNotNull(doc, "Document was null");
            Assert.AreEqual(buf.Length, read);
            Assert.IsTrue(doc.Contains("a"));
            
        }

        
        [Test]
        public void TestReadBigDocument(){
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            
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
            ;
            writer.WriteDocument(expected);
            writer.Flush();
            ms.Seek(0,SeekOrigin.Begin);           
            
            BsonReader2 reader = new BsonReader2(ms);
            Document doc = reader.Read();
            
            Assert.IsNotNull(doc);
        }
        
        
        private String ConvertDocToHex(Document doc){
            MemoryStream ms = new MemoryStream();
            BsonWriter2 writer = new BsonWriter2(ms);
            
            writer.WriteDocument(doc);
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
