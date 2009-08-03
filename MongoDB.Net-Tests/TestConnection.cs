
using System;
using NUnit.Framework;

using MongoDB.Driver;
using MongoDB.Driver.IO;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
	
	
	[TestFixture()]
	public class TestConnection
	{
		
//		[Test()]
//		public void TestSendMsgMessage(){
//			Connection conn = new Connection("10.141.153.2");
//			//Connection conn = new Connection();
//			conn.Open();
//			conn.SendMsgMessage("Hello MongoDB!");
//			conn.SendMsgMessage("Hello MongoDB2!");
//			conn.SendMsgMessage("Hello MongoDB3!");
//			conn.SendMsgMessage("Hello MongoDB4!");
//			conn.SendMsgMessage("Hello MongoDB5!");
//			conn.SendMsgMessage("Hello MongoDB6!");
//			conn.SendMsgMessage("Hello MongoDB7!");
//			conn.SendMsgMessage("Hello MongoDB8!");
//			conn.Close();
//		}
		
		[Test]
		public void TestSendQueryMessage(){
			//Connection conn = new Connection("10.141.153.2");
			Connection conn = new Connection();
			conn.Open();
			
			BsonDocument qdoc = new BsonDocument();
			qdoc.Add("listDatabases", new BsonNumber(1.0));
			//QueryMessage qmsg = new QueryMessage(qdoc,"system.namespaces");
			QueryMessage qmsg = new QueryMessage(qdoc,"admin.$cmd");
			qmsg.NumberToReturn = -1;
			conn.SendTwoWayMessage(qmsg);
			
			conn.Close();
		}
	}
}
