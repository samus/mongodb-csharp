using System.Configuration;

using NUnit.Framework;

namespace MongoDB.Driver.Configuration
{
    [TestFixture]
    public class TestConfigurationSection
    {
        [Test]
        public void TestReadNamed(){
            MongoConfigurationSection config = (MongoConfigurationSection)ConfigurationManager.GetSection("Mongo");
            Assert.AreEqual("Server=localhost:27018", config.Connections["local21018"].ConnectionString);
        }
        
        [Test]
        public void TestReadDefaults(){
            MongoConfigurationSection config = (MongoConfigurationSection)ConfigurationManager.GetSection("Mongo");
            Assert.AreEqual("Server=localhost:27017", config.Connections["defaults"].ConnectionString);
        }
    }
}
