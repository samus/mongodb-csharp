using System;
using System.IO;
using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonBinary
    {
        protected static byte[] DecodeHex(string val){
            var numberChars = val.Length;

            var bytes = new byte[numberChars/2];
            for(var i = 0; i < numberChars; i += 2)
                try{
                    bytes[i/2] = Convert.ToByte(val.Substring(i, 2), 16);
                }
                catch{
                    //failed to convert these 2 chars, they may contain illegal charracters
                    bytes[i/2] = 0;
                }
            return bytes;
        }

        [Test]
        public void TestBinaryRead(){
            const string hex = "28000000075f6964004b1971811d8b0f00c0000000056461746100070000000203000000e188b400";

            var data = DecodeHex(hex);
            var inmem = new MemoryStream(data);
            var inreader = new BsonReader(inmem);
            var indoc = inreader.Read();

            var outmem = new MemoryStream();
            var outwriter = new BsonWriter(outmem);
            outwriter.WriteObject(indoc);
            var outdata = outmem.ToArray();
            var outhex = BitConverter.ToString(outdata);
            outhex = outhex.Replace("-", "");

            Assert.AreEqual(hex, outhex.ToLower());
        }

        [Test]
        public void TestRoundTrip(){
            var idoc = new Document{{"b", new Binary(new[]{(byte)1, (byte)2})}};

            var stream = new MemoryStream();
            var writer = new BsonWriter(stream);
            writer.WriteObject(idoc);

            stream.Seek(0, SeekOrigin.Begin);
            var reader = new BsonReader(stream);
            var odoc = reader.Read();

            Assert.AreEqual(idoc.ToString(), odoc.ToString());
        }
    }
}