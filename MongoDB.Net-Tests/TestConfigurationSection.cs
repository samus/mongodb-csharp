using System;
using NUnit.Framework;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Configuration;
using System.Text;

namespace MongoDB.Driver.Tests
{
    [TestFixture]
    public class TestConfigurationSection
    {
        [Test]
        public void TestReadConfig(){

            MongoDBConfiguration config = (MongoDBConfiguration)System.Configuration.ConfigurationManager.GetSection("MongoDB");
            Assert.AreEqual("servername", config.Connection.Host);
            Assert.AreEqual(27017, config.Connection.Port);
            Assert.AreEqual("test", config.Connection.Database);
            Assert.AreEqual("testFS", config.GridFS.Collection);
            Assert.AreEqual(512000, config.GridFS.ChunkSize);
       }

    }
}
