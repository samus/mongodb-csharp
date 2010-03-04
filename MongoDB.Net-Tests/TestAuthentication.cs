/*
 *  User: Sedward
 */
using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture(Description = "Requires start server with --auth")]
	public class TestAuthentication
	{
        const String testDatabase = "testAuth";
	    const String testuser = "testuser";
        const String testpass = "test1234";

        const String adminuser = "adminuser";
        const String adminpass = "admin1234";

        [TestFixtureSetUp]
        public void SetUp()
        {
            using(var mongo = new Mongo())
            {
                mongo.Connect();

                try
                {
                    mongo["admin"].MetaData.AddUser(adminuser, adminpass);
                }
                catch(MongoException) { }

                try
                {
                    mongo[testDatabase].MetaData.AddUser(testuser, testpass);
                }
                catch(MongoException){}
            }
        }

	    [Test]
        public void TestLoginGoodPassword()
	    {
            using(var mongo = ConnectAndAuthenticatedMongo(testuser, testpass, MongoServerEndPoint.DefaultPort))
	            TryInsertData(mongo);
	    }

	    [Test]
        [ExpectedException(typeof(MongoException))]
        public void TestLoginBadPassword()
        {
            using(var mongo = ConnectAndAuthenticatedMongo(testuser, "badpassword", MongoServerEndPoint.DefaultPort))
                TryInsertData(mongo);
        }

	    [Test]
        public void TestAuthenticatedInsert(){
            using(var mongo = ConnectAndAuthenticatedMongo(testuser, testpass,MongoServerEndPoint.DefaultPort))
                TryInsertData(mongo);
        }

	    [Test]
        [ExpectedException(typeof(MongoOperationException))]
        public void TestUnauthenticatedInsert(){
            using(var mongo = new Mongo())
            {
                mongo.Connect();

                TryInsertData(mongo);
            }
        }

	    private static Mongo ConnectAndAuthenticatedMongo(string username,string password,int port)
        {
            var builder = new MongoConnectionStringBuilder {Username = username, Password = password};
            builder.AddServer("localhost",port);
            var mongo = new Mongo(builder.ToString());
	        mongo.Connect();
	        return mongo;
        }

	    private static void TryInsertData(Mongo mongo)
	    {
            var collection = mongo[testDatabase]["testCollection"];
            collection.Delete(new Document(),true);
            collection.Insert(new Document().Append("value", 84),true);
            
            var value = collection.FindOne(new Document().Append("value", 84));
            
            Assert.AreEqual(84, value["value"]);
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
            using(var mongo = ConnectAndAuthenticatedMongo(adminuser, adminuser))
            {
                mongo[testDatabase].MetaData.RemoveUser(testuser);
                mongo["admin"].MetaData.RemoveUser(adminuser);
            }
	    }
	}
}