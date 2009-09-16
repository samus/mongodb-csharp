
using System;
using System.IO;

using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
    
    
    [TestFixture]
    public class TestBsonCode
    {
        
        [Test]
        public void TestConvertFrom(){
            string val = "return 2;";
            Code code = new Code(val);
            BsonCode bcode = BsonConvert.From(code);
            Assert.AreEqual(val, bcode.Val);
        }
        [Test]
        public void TestFormatting(){
            BsonCode bcode = new BsonCode();
            bcode.Val = "return 2;";
            MemoryStream buf = new MemoryStream();
            BsonWriter writer = new BsonWriter(buf);
            bcode.Write(writer);
            writer.Flush();
            Byte[] output = buf.ToArray();
            String hexdump = BitConverter.ToString(output);
            hexdump = hexdump.Replace("-","");
            Assert.AreEqual(10,output[0],"Size didn't take into count null terminator");
            Assert.AreEqual("0A00000072657475726E20323B00",hexdump, "Dump not correct");
        }
        
        [Test]
        public void TestSize(){
            BsonCode bcode = new BsonCode();
            bcode.Val = "return 2;";
            Assert.AreEqual(14,bcode.Size);
        }
        
    }
}
