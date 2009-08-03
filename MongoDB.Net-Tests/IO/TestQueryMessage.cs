/*
 * User: scorder
 * Date: 7/15/2009
 */

using System;
using System.IO;

using NUnit.Framework;

using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
	[TestFixture]
	public class TestQueryMessage
	{
		[Test]
		public void TestAllBytesWritten()
		{
			BsonDocument query = new BsonDocument();
			query.Add("col1", BsonConvert.From(1));
			
			QueryMessage msg = new QueryMessage(query,"TestDB.TestCol");
			MemoryStream buffer = new MemoryStream();
			msg.Write(buffer);
			
			Byte[] output = buffer.ToArray();
			String hexdump = BitConverter.ToString(output);
			//Console.WriteLine("Dump: " + hexdump);

			Assert.IsTrue(output.Length > 0);
			Assert.AreEqual("3A-00-00-00-00-00-00-00-00-00-00-00-D4-07-00-00-00-00-00-00-54-65-73-74-44-42-2E-54-65-73-74-43-6F-6C-00-00-00-00-00-00-00-00-00-0F-00-00-00-10-63-6F-6C-31-00-01-00-00-00-00", hexdump);
			

		}
	}
}
