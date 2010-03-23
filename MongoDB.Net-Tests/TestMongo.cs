using System;
using System.Configuration;

using NUnit.Framework;

using MongoDB.Driver;
    
namespace MongoDB.Driver
{
    [TestFixture()]
    public class TestMongo
    {
        string connectionString = ConfigurationManager.AppSettings["tests"];       
        [Test()]
        public void TestDefaults()
        {
            Mongo m = new Mongo(); //Connection string not needed since connect not called and it would screw up the test.
            Assert.AreEqual(string.Empty, m.ConnectionString);
        }
        
        [Test()]
        public void TestExplicitConnection(){
            Mongo m = new Mongo(connectionString);
            Assert.IsTrue(m.Connect());
        }
        
        [Test()]
        public void TestThatConnectMustBeCalled(){
            Mongo m = new Mongo(connectionString);
            bool thrown = false;
            try{
                MongoDatabase db = m["admin"];
                db["$cmd"].FindOne(new Document().Add("listDatabases", 1.0));
            }catch(MongoCommException){
                thrown = true;
            }
            Assert.IsTrue(thrown, "MongoComException not thrown");
        }
    }
}
