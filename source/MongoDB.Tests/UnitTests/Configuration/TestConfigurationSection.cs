using MongoDB.Configuration;
using NUnit.Framework;

namespace MongoDB.UnitTests.Configuration
{
    [TestFixture]
    public class TestConfigurationSection
    {
        [Test]
        public void TestReadDefaults()
        {
            var config = MongoConfigurationSection.GetSection();
            Assert.AreEqual("Server=localhost:27017", config.Connections["defaults"].ConnectionString);
        }

        [Test]
        public void TestReadNamed()
        {
            var config = MongoConfigurationSection.GetSection();
            Assert.AreEqual("Server=localhost:27018", config.Connections["local21018"].ConnectionString);
        }
    }
}