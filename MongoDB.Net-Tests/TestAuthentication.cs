/*
 *  User: Sedward
 */
using System;
using MongoDB.Driver.Connections;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture(Description = "Requires start server with --auth")]
	public class TestAuthentication
	{
        private const int AuthServerPort = MongoServerEndPoint.DefaultPort + 3;
        const String testDatabaseName = "testAuth";
	    const String testuser = "testuser";
        const String testpass = "test1234";

        const String adminuser = "adminuser";
        const String adminpass = "admin1234";

        [TestFixtureSetUp]
        public void SetUp()
        {
            using(var mongo = new Mongo(CreateConnectionStringBuilder().ToString()))
            {
                mongo.Connect();

                var testDatabase = mongo[testDatabaseName];
                if(testDatabase.MetaData.FindUser(testuser) == null)
                    testDatabase.MetaData.AddUser(testuser, testpass);

                var adminDatabase = mongo["admin"];
                if(adminDatabase.MetaData.FindUser(adminuser) == null)
                    adminDatabase.MetaData.AddUser(adminuser, adminpass);
            }
        }

	    [Test]
        public void TestLoginGoodPassword()
	    {
            using(var mongo = ConnectAndAuthenticatedMongo(testuser, testpass))
	            TryInsertData(mongo);
	    }

	    [Test]
        [ExpectedException(typeof(MongoException))]
        public void TestLoginBadPassword()
        {
            using(var mongo = ConnectAndAuthenticatedMongo(testuser, "badpassword"))
                TryInsertData(mongo);
        }

	    [Test]
        public void TestAuthenticatedInsert(){
            using(var mongo = ConnectAndAuthenticatedMongo(testuser, testpass))
                TryInsertData(mongo);
        }

	    [Test]
        [ExpectedException(typeof(MongoOperationException))]
        public void TestUnauthenticatedInsert(){
            using(var mongo = new Mongo(CreateConnectionStringBuilder().ToString()))
            {
                mongo.Connect();

                TryInsertData(mongo);
            }
        }

	    private static Mongo ConnectAndAuthenticatedMongo(string username,string password)
        {
	        var builder = CreateConnectionStringBuilder();
            builder.Username = username;
	        builder.Password = password;
            var mongo = new Mongo(builder.ToString());
	        mongo.Connect();
	        return mongo;
        }

        private static MongoConnectionStringBuilder CreateConnectionStringBuilder()
        {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("localhost", AuthServerPort);
            return builder;
        }

	    private static void TryInsertData(Mongo mongo)
	    {
            var collection = mongo[testDatabaseName]["testCollection"];
            collection.Delete(new Document(),true);
            collection.Insert(new Document().Add("value", 84),true);
            
            var value = collection.FindOne(new Document().Add("value", 84));
            
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
                mongo[testDatabaseName].MetaData.RemoveUser(testuser);
                mongo["admin"].MetaData.RemoveUser(adminuser);
            }

            // clean connections
            ConnectionFactory.Shutdown();
	    }
	}
}