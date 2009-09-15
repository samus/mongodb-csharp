/*
 *  User: Sedward
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Bson;
using MongoDB.Driver.IO;

namespace MongoDB.Driver
{
	[TestFixture]
	public class Authentication
	{
	    [Test]
        public void TestLoginGoodPassword(){
            Connection conn = new Connection("127.0.0.1", 27017);
            Database db = new Database(conn, "TestAuth");
            conn.Open();
            bool ok = db.Authenticate("testuser", "test1234");
            Assert.IsTrue(ok);
            db.Logout();
            conn.Close();
        }

        [Test]
        public void TestLoginBadPassword(){
            Connection conn = new Connection("127.0.0.1", 27017);
            Database db = new Database(conn, "TestAuth");
            conn.Open();
            bool ok = db.Authenticate("testuser", "badpassword");
            Assert.IsFalse(ok);
            conn.Close();
        }

        [Test]
        public void TestAdminLogin(){
            Connection conn = new Connection("127.0.0.1", 27017);
            Database db = new Database(conn, "admin");
            conn.Open();
            bool ok = db.Authenticate("adminuser", "admin1234");
            Assert.IsTrue(ok);
            conn.Close();
        }

        [Test]
        public void TestAuthenticatedInsert(){
            Connection conn = new Connection("127.0.0.1", 27017);
            conn.Open();
            Database db = new Database(conn, "TestAuth");
            bool ok = db.Authenticate("testuser", "test1234");
            Collection tests = db["inserts"];
            if (ok)
            {                
                tests.Insert(new Document().Append("value", 34));
            }
            Document valA = tests.FindOne(new Document().Append("value", 34));
            Assert.AreEqual(34, valA["value"]);
            db.Logout();
            conn.Close();
        }

        [Test]
        public void TestUnauthenticatedInsert(){
            Connection conn = new Connection("127.0.0.1", 27017);
            conn.Open();
            Database db = new Database(conn, "TestAuth");
            Collection tests = db["inserts"];
            tests.Insert(new Document().Append("value", 84));
            Document valA = tests.FindOne(new Document().Append("value", 84));
            Assert.AreNotEqual(84, valA["value"]);
            conn.Close();
        }

        [Test]
        public void TestLoggedOutInsert()
        {
            Connection conn = new Connection("127.0.0.1", 27017);
            conn.Open();
            Database db = new Database(conn, "TestAuth");
            bool ok = db.Authenticate("testuser", "test1234");
            Collection tests = db["inserts"];
            if (ok)
            {
                db.Logout();
                tests.Insert(new Document().Append("value", 47));
            }
            Document valA = tests.FindOne(new Document().Append("value", 47));
            Assert.AreNotEqual(47, valA["value"]);
            conn.Close();
        }

        [TestFixtureSetUp]
        public void TestSetUp(){
            Connection conn = new Connection("127.0.0.1", 27017);
            conn.Open();
            Database admindb = new Database(conn, "admin");
            admindb.AddUser("adminuser", "admin1234");
            bool ok = admindb.Authenticate("adminuser", "admin1234");
            if (ok){
                Database testAuth = new Database(conn, "TestAuth");
                testAuth.AddUser("testuser", "test1234");
            }
            conn.Close();
        }

        [TestFixtureTearDown]
        public void TestTearDown(){
            Connection conn = new Connection("127.0.0.1", 27017);
            conn.Open();
            Database testAuth = new Database(conn, "TestAuth");
            bool ok = testAuth.Authenticate("testuser", "test1234");
            if (ok)
            {
                Collection cmd = testAuth["$cmd"];
                cmd.FindOne(new Document().Append("drop", "inserts"));
            }
            Collection testAuthUsers = testAuth.GetCollection("system.users");
            testAuthUsers.Delete(new Document().Append("user", "testuser"));
                   
            Database admin = new Database(conn, "admin");
            bool ok2 = admin.Authenticate("adminuser", "admin1234");
            if (ok2){
               Collection adminUsers = admin.GetCollection("system.users");
               adminUsers.Delete(new Document().Append("user", "adminuser"));
            }
        }



	}
}
