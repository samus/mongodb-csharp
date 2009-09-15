
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
            Assert.Fail("Write Test");
        }
    }
}
