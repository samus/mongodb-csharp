using System;
using System.Configuration;

using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.Driver.Configuration
{
    [TestFixture]
    public class TestConfigurationSection
    {
        [Test]
        public void TestReadNamed(){
            MongoConfiguration config = (MongoConfiguration)ConfigurationManager.GetSection("Mongo");
            Assert.AreEqual("Server=localhost:27018", config.Connections["local21018"].ConnectionString);
        }
        
        [Test]
        public void TestReadDefaults(){
            MongoConfiguration config = (MongoConfiguration)ConfigurationManager.GetSection("Mongo");
            Assert.AreEqual("Server=localhost:27017", config.Connections["defaults"].ConnectionString);
        }
    }
}
