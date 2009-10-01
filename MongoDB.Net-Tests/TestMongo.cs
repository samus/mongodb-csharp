
using System;

using NUnit.Framework;

using MongoDB.Driver;
    
namespace MongoDB.Driver
{
    
    
    [TestFixture()]
    public class TestMongo
    {
        
        [Test()]
        public void TestDefaults()
        {
            Mongo m = new Mongo();
            Assert.AreEqual("localhost", m.Host);
            Assert.AreEqual(27017, m.Port);
        }
        
        [Test()]
        public void TestExplicitConnection(){
            Mongo m = new Mongo();
            Assert.IsTrue(m.Connect());
        }
        
        [Test()]
        public void TestThatConnectMustBeCalled(){
            Mongo m = new Mongo();
            bool thrown = false;
            try{
                Database db = m["admin"];
                db["$cmd"].FindOne(new Document().Append("listDatabases", 1.0));
            }catch(MongoCommException){
                thrown = true;
            }
            Assert.IsTrue(thrown, "MongoComException not thrown");
        }
    }
}
