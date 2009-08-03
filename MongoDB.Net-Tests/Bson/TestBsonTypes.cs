/*
 * User: scorder
 * Date: 7/20/2009
 */

using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.Bson
{
	[TestFixture]
	public class TestBsonTypes
	{
		MemoryStream mem;
		BsonReader reader;
		BsonWriter writer;
		
		[Test]
		public void TestBsonInteger(){
			InitStreams();
			BsonInteger w = new BsonInteger(5);
			w.Write(writer);
			
			FlushAndGotoBegin();
			
			BsonInteger r = new BsonInteger();
			r.Read(reader);
			
			Assert.AreEqual(w.Val, r.Val);
		}
		
		[Test]
		public void TestBsonBoolean(){
			InitStreams();
			BsonBoolean w = new BsonBoolean(true);
			w.Write(writer);
			
			FlushAndGotoBegin();
			
			BsonBoolean r = new BsonBoolean();
			r.Read(reader);
			
			Assert.AreEqual(w.Val, r.Val);
		}

		[Test]
		public void TestBsonString(){
			InitStreams();
			BsonString w = new BsonString("test");
			w.Write(writer);
			
			FlushAndGotoBegin();
			
			BsonString r = new BsonString();
			r.Read(reader);
			
			Assert.AreEqual(w.Val, r.Val);
		}
		
		
//		[Test]
//		public void TestBsonInteger(){
//			InitStreams();
//			BsonInteger w = new BsonInteger(5);
//			w.Write(writer);
//			
//			FlushAndGotoBegin();
//			
//			BsonInteger r = new BsonInteger();
//			r.Read(reader);
//			
//			Assert.AreEqual(w.Val, r.Val);
//		}
		
		
		protected void InitStreams(){
			mem = new MemoryStream();
			reader = new BsonReader(mem);
			writer = new BsonWriter(mem);					
		}
		
		protected void FlushAndGotoBegin(){
			writer.Flush();
			mem.Seek(0,SeekOrigin.Begin);			
		}		
	}
}
