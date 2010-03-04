/*
 *  User: Sedward
 */
using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
	[TestFixture]
	public class TestAuthentication
	{
        Mongo mongo = new Mongo();
        
        Database db;
        String testuser = "testuser";
        String testpass = "test1234";
        
        Database admindb;
        String adminuser = "adminuser";
        String adminpass = "admin1234";
        
	    [Test]
        public void TestLoginGoodPassword(){
            bool ok = Authenticate();
            Assert.IsTrue(ok);
            db.Logout();
        }

        [Test]
        public void TestLoginBadPassword(){
            bool ok = db.Authenticate("testuser", "badpassword");
            Assert.IsFalse(ok);
        }

        [Test]
        public void TestAdminLogin(){
            bool ok = admindb.Authenticate(adminuser, adminpass);
            Assert.IsTrue(ok);
            admindb.Logout();
        }

        [Test]
        public void TestAuthenticatedInsert(){
            bool ok = Authenticate();
            IMongoCollection tests = db["inserts"];
            if (ok){
                tests.Insert(new Document().Append("value", 34));
            }
            Document valA = tests.FindOne(new Document().Append("value", 34));
            Assert.AreEqual(34, valA["value"]);
            db.Logout();
        }

        [Test]
        public void TestUnauthenticatedInsert(){
            try{
                db.Logout();
            }catch(MongoException){
                //We don't care.  Just wanted to make sure we weren't logged in
            }
            IMongoCollection tests = db["inserts"];
            tests.Insert(new Document().Append("value", 84));
            Document valA = tests.FindOne(new Document().Append("value", 84));
            Assert.AreNotEqual(84, valA["value"]);
        }
        
        [TestFixtureSetUp]
        public void TestSetUp(){
            mongo.Connect();
            db = mongo["TestAuth"];
            admindb = mongo["admin"];

            admindb.MetaData.AddUser(adminuser, adminpass);
            admindb.Authenticate(adminuser, adminpass);
            db.MetaData.AddUser(testuser, testpass);
            admindb.Logout();
        }

        [TestFixtureTearDown]
        public void TestTearDown(){
            /*
             * In case clean up fails open a Mongo shell and execute the following commands
             * use admin
             * db.auth("adminuser", "admin1234");
             * db.system.users.find(); //should see adminuser
             * db.system.users.remove({user:"adminuser"});
             * db.system.users.find(); //should not see adminuser or any other.
             * Tests should now run.
             */
            Authenticate();
            admindb.Authenticate(adminuser, adminpass);
            
            db.MetaData.DropCollection("inserts");
            
            db.MetaData.RemoveUser(testuser);
            admindb.MetaData.RemoveUser(adminuser);
        }

        protected bool Authenticate(){
            return db.Authenticate(testuser,testpass);
        }
    }
}
//error: {"$err" : "unauthorized"} incorporate sometime.