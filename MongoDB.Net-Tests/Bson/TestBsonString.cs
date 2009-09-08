/*
 * User: scorder
 * Date: 7/15/2009
 */

using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonString
    {
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
    }
}
