using System;
using System.Configuration;
using MongoDB.Connections;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.SecondServer
{
    /// <remarks>
    /// In case clean up fails open a Mongo shell and execute the following commands
    /// * use admin
    /// * db.auth("adminuser", "admin1234");
    /// * db.system.users.find(); //should see adminuser
    /// * db.system.users.remove({user:"adminuser"});
    /// * db.system.users.find(); //should not see adminuser or any other.
    /// * Tests should now run.
    /// </remarks>
    [Ignore("Run manually since it needs a second server with --auth")]
    [TestFixture]
	public class TestAuthentication
	{
        private readonly string _connectionString;

        public TestAuthentication()
        {
            _connectionString = ConfigurationManager.AppSettings["auth"];
        }

        const String TestDatabaseName = "testAuth";
	    const String TestUser = "testuser";
        const String TestPass = "test1234";

        const String AdminUser = "adminuser";
        const String AdminPass = "admin1234";

        [TestFixtureSetUp]
        public void SetUp()
        {
            using(var mongo = new Mongo(_connectionString))
            {
                mongo.Connect();

                var testDatabase = mongo[TestDatabaseName];
                if(testDatabase.Metadata.FindUser(TestUser) == null)
                    testDatabase.Metadata.AddUser(TestUser, TestPass);

                var adminDatabase = mongo["admin"];
                if(adminDatabase.Metadata.FindUser(AdminUser) == null)
                    adminDatabase.Metadata.AddUser(AdminUser, AdminPass);
            }
        }

	    [Test]
        public void TestLoginGoodPassword()
	    {
            using(var mongo = ConnectAndAuthenticatedMongo(TestUser, TestPass))
	            TryInsertData(mongo);
	    }

	    [Test]
        [ExpectedException(typeof(MongoException))]
        public void TestLoginBadPassword()
        {
            using(var mongo = ConnectAndAuthenticatedMongo(TestUser, "badpassword"))
                TryInsertData(mongo);
        }

	    [Test]
        public void TestAuthenticatedInsert(){
            using(var mongo = ConnectAndAuthenticatedMongo(TestUser, TestPass))
                TryInsertData(mongo);
        }

	    [Test]
        [ExpectedException(typeof(MongoOperationException))]
        public void TestUnauthenticatedInsert(){
            using(var mongo = new Mongo(_connectionString))
            {
                mongo.Connect();

                TryInsertData(mongo);
            }
        }

	    private Mongo ConnectAndAuthenticatedMongo(string username,string password)
        {
	        var builder = new MongoConnectionStringBuilder(_connectionString)
	        {
	            Username = username, 
                Password = password
	        };
	        var mongo = new Mongo(builder.ToString());
	        mongo.Connect();
	        return mongo;
        }

	    private static void TryInsertData(Mongo mongo)
	    {
            var collection = mongo[TestDatabaseName]["testCollection"];
            collection.Delete(new Document(),true);
            collection.Insert(new Document().Add("value", 84),true);
            
            var value = collection.FindOne(new Document().Add("value", 84));
            
            Assert.AreEqual(84, value["value"]);
        }

	    [TestFixtureTearDown]
	    public void TestTearDown(){
            using(var mongo = ConnectAndAuthenticatedMongo(AdminUser, AdminUser))
            {
                mongo[TestDatabaseName].Metadata.RemoveUser(TestUser);
                mongo["admin"].Metadata.RemoveUser(AdminUser);
            }

            // clean connections
            ConnectionFactory.Shutdown();
	    }
	}
}