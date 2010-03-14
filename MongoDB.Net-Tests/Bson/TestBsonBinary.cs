using System;
using System.IO;
using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
    [TestFixture]
    public class TestBsonBinary
    {
        [Test]
        public void TestRoundTrip ()
        {
            Document idoc = new Document ();
            idoc.Add ("b", new Binary (new byte[] { (byte)1, (byte)2 }));
            
            MemoryStream stream = new MemoryStream ();
            BsonWriter writer = new BsonWriter (stream);
            writer.Write (idoc);
            
            stream.Seek (0, SeekOrigin.Begin);
            BsonReader reader = new BsonReader (stream);
            Document odoc = reader.Read ();
            
            Assert.AreEqual (idoc.ToString (), odoc.ToString ());
        }

        [Test]
        public void TestBinaryRead ()
        {
            string hex = "28000000075f6964004b1971811d8b0f00c0000000056461746100070000000203000000e188b400";
            
            byte[] data = DecodeHex (hex);
            MemoryStream inmem = new MemoryStream (data);
            BsonReader inreader = new BsonReader (inmem);
            Document indoc = new Document ();
            indoc = inreader.Read ();
            
            MemoryStream outmem = new MemoryStream ();
            BsonWriter outwriter = new BsonWriter (outmem);
            outwriter.Write (indoc);
            byte[] outdata = outmem.ToArray ();
            String outhex = BitConverter.ToString (outdata);
            outhex = outhex.Replace ("-", "");
            
            Assert.AreEqual (hex, outhex.ToLower ());
            
        }

        protected static byte[] DecodeHex (string val)
        {
            int numberChars = val.Length;
            
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2) {
                try {
                    bytes[i / 2] = Convert.ToByte (val.Substring (i, 2), 16);
                } catch {
                    //failed to convert these 2 chars, they may contain illegal charracters
                    bytes[i / 2] = 0;
                }
            }
            return bytes;
        }
        
    }
}
