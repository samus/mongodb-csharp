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
            Assert.AreEqual("servername", config.Host);
            Assert.AreEqual(27017, config.Port);
            Assert.AreEqual("testcollection", config.Collection);
       }

    }
}
