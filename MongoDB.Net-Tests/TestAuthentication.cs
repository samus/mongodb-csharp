/*
 *  User: Sedward
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using MongoDB.Driver;
using MongoDB.Driver.Bson;
using MongoDB.Driver.IO;

namespace MongoDB.Driver
{
	[TestFixture]
	public class Authentication
	{
		//[Test]
		//public void TestAddUser()
		//{
		//    Mongo mongo = new Mongo();
		//    mongo.Connect();
		//    Collection
		//    Assert.IsNotNull(db);
		//    db.AddUser("sed", "test");
		//}

		//[Test]
		//public void TestUserAdded()
		//{
		//    Mongo db = new Mongo();
		//    db.Connect();
		//    Collection c = db["system"]["users"];
		//    Document result = c.FindOne(new Document().Append("username", "sed"));
		//    Assert.IsNotNull(result);

		//}

		[Test]
		public void AuthenticateTest()
		{
			double value = 1.0;
			Assert.AreEqual(1, value);		
		}

		/// <summary>
		/// To perform this test the user needs to be added useing the shell
		/// Server must be started with --auth flag
		/// </summary>
		[Test]
		public void TestAuthenticationGoodPassword(){
			Connection conn = new Connection("127.0.0.1", 27017);
			Database db = new Database(conn, "tests");
			bool ok = db.Authenticate("seth", "test");
			Assert.IsTrue(ok);
		}

		[Test]
		public void TestAuthenticationBadPassword(){
			Connection conn = new Connection("127.0.0.1", 27017);
			Database db = new Database(conn, "tests");
			bool ok = db.Authenticate("seth", "badpassword");
			Assert.IsFalse(ok);
		}


	}
}
