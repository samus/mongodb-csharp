/*
 * User: scorder
 * Date: 7/15/2009
 */
using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
	/// <summary>
	/// Description of TestBsonDocument.
	/// </summary>
	[TestFixture()]
	public class TestBsonDocument
	{
		[Test()]
		public void TestAdds(){
			BsonDocument doc = new BsonDocument();
			for(int i = 1; i < 6; i++){
				BsonElement be = new BsonElement(Convert.ToString(i),new BsonInteger(i));
				doc.Add(be.Name, be);
			}
			Assert.AreEqual(5,doc.Count,"Not all elements there");
			
			doc.Add(new BsonElement("6", new BsonInteger(6)));
			Assert.AreEqual(6,doc.Count,"Not all elements there");
			
			doc.Add("7", new BsonInteger(7));
			Assert.AreEqual(7,doc.Count,"Not all elements there");
			
		}
		
		[Test]
		public void TestSize(){
			BsonDocument doc = new BsonDocument();
			Assert.AreEqual(5, doc.Size);
			doc.Add("test", new BsonString("test"));
			Assert.AreEqual(20, doc.Size);
			for(int i = 1; i < 6; i++){
				doc.Add(Convert.ToString(i),new BsonInteger(i));
			}
			Assert.AreEqual(55, doc.Size);
			
			BsonDocument sub = new BsonDocument();
			sub.Add("test", new BsonString("sub test"));
			Assert.AreEqual(24,sub.Size);
			doc.Add("sub",sub);
			Assert.AreEqual(84,doc.Size);
		}
		
		[Test]
		public void TestFormatting(){
			BsonDocument doc = new BsonDocument();
			MemoryStream buf = new MemoryStream();
			BsonWriter writer = new BsonWriter(buf);
			
			
			doc.Add("test", new BsonString("test"));
			doc.Write(writer);
			writer.Flush();
			
			Byte[] output = buf.ToArray();
			String hexdump = BitConverter.ToString(output);
			hexdump = hexdump.Replace("-","");
			Assert.AreEqual(20,output[0],"Size didn't take into count null terminator");
			Assert.AreEqual("1400000002746573740005000000746573740000",hexdump, "Dump not correct");
		}		
		
		[Test]
		public void TestElements(){
			BsonDocument bdoc = new BsonDocument();
			MemoryStream buf = new MemoryStream();
			BsonWriter writer = new BsonWriter(buf);
			
			Oid oid = new Oid("4a753ad8fac16ea58b290351");
			
			bdoc.Append("_id", new BsonElement("_id",new BsonOid(oid)))
				.Append("a", new BsonElement("a",new BsonNumber(1)))
				.Append("b", new BsonElement("b",new BsonString("test")));
			bdoc.Write(writer);


			writer.Flush();
			
			Byte[] output = buf.ToArray();
			String hexdump = BitConverter.ToString(output);
			hexdump = hexdump.Replace("-","");
							 //0         1         2         3         4         5         6         7         8         9
			                 //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
			string expected = "2D000000075F6964004A753AD8FAC16EA58B290351016100000000000000F03F02620005000000746573740000";
			Assert.AreEqual(expected,hexdump, "Dump not correct");
		}
		
		[Test]
		public void TestNumberElements(){
			BsonDocument bdoc = new BsonDocument();
			MemoryStream buf = new MemoryStream();
			BsonWriter writer = new BsonWriter(buf);
			
			Oid oid = new Oid("4a75384cfac16ea58b290350");
			
			bdoc.Append("_id", new BsonElement("_id",new BsonOid(oid)))
				.Append("a", new BsonElement("a",new BsonNumber(1)))
				.Append("b", new BsonElement("b",new BsonNumber(2)));
			bdoc.Write(writer);


			writer.Flush();
			
			Byte[] output = buf.ToArray();
			String hexdump = BitConverter.ToString(output);
			hexdump = hexdump.Replace("-","");
			
			Assert.AreEqual("2C000000075F6964004A75384CFAC16EA58B290350016100000000000000F03F016200000000000000004000",hexdump, "Dump not correct");
		}
		
	}
}
