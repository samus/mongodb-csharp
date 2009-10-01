/*
 * User: scorder
 * Date: 7/15/2009
 */

using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonString
    {
        private String empty;
        [Test]
        public void TestFormatting(){
            BsonString str = new BsonString("test");
            MemoryStream buf = new MemoryStream();
            BsonWriter writer = new BsonWriter(buf);
            str.Write(writer);
            writer.Flush();
            Byte[] output = buf.ToArray();
            String hexdump = BitConverter.ToString(output);
            hexdump = hexdump.Replace("-","");
            Assert.AreEqual(5,output[0],"Size didn't take into count null terminator");
            Assert.AreEqual("050000007465737400",hexdump, "Dump not correct");
        }
        
        [Test]
        public void TestSize(){
            BsonString str = new BsonString("test");
            Assert.AreEqual(9,str.Size);
        }
        
        [Test]
        public void TestSizeWithUKPound(){
            UTF8Encoding encoding = new UTF8Encoding();
            int baseSize = 5;
            String test = "1£";
            BsonString bstr = new BsonString(test);
            Assert.AreEqual(baseSize + encoding.GetByteCount(test), bstr.Size, "Size didn't count the double wide pound symbol correctly");
        }
        
        [Test]
        public void TestFormattingWithUKPound(){
            BsonString str = new BsonString("1234£56");
            MemoryStream buf = new MemoryStream();
            BsonWriter writer = new BsonWriter(buf);
            str.Write(writer);
            writer.Flush();
            Byte[] output = buf.ToArray();
            String hexdump = BitConverter.ToString(output);
            hexdump = hexdump.Replace("-","");
            Assert.AreEqual("0900000031323334C2A3353600",hexdump, "Dump not correct");
            Assert.AreEqual(9,output[0],"Size didn't take into account size of pound symbol");
        }
    }
}
