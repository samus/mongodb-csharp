
using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
	[TestFixture()]
	public class TestBsonRegex
	{
		[Test]
		public void TestFormatting(){
			BsonRegex reg = new BsonRegex("test");
			MemoryStream buf = new MemoryStream();
			BsonWriter writer = new BsonWriter(buf);
			reg.Write(writer);
			writer.Flush();
			Byte[] output = buf.ToArray();
			String hexdump = BitConverter.ToString(output);
			hexdump = hexdump.Replace("-","");
			Assert.AreEqual("74657374000",hexdump, "Dump not correct");
		}		
		
		[Test]
		public void TestSize(){
			BsonRegex reg = new BsonRegex("test");
			Assert.AreEqual(6,reg.Size);
			
			reg = new BsonRegex("test", "option");
			Assert.AreEqual(12, reg.Size);
		}
		
	 	//TODO: Add tests to test option values and what is and isn't allowed.
	}
}
