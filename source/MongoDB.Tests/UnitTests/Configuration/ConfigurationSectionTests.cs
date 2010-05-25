using System.Configuration;
using System.IO;
using MongoDB.Configuration;
using NUnit.Framework;

namespace MongoDB.UnitTests.Configuration
{
    [TestFixture]
    public class ConfigurationSectionTests
    {
        private static MongoConfigurationSection ReadFromFile(string name)
        {
            return ReadFromFile(name, MongoConfigurationSection.DefaultSectionName);
        }

        private static MongoConfigurationSection ReadFromFile(string name, string section)
        {
            var map = new ExeConfigurationFileMap {ExeConfigFilename = Path.Combine("Test-Sections", name)};
            var exeConfiguration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            return exeConfiguration.GetSection(section) as MongoConfigurationSection;
        }

        [Test]
        public void CanReadFromTestsConfig()
        {
            var config = MongoConfigurationSection.GetSection();
            Assert.IsNotNull(config);
            Assert.AreEqual("Server=localhost:27017", config.Connections["default"].ConnectionString);
            Assert.AreEqual("Server=localhost:27018", config.Connections["local21018"].ConnectionString);
        }

        [Test]
        public void CanReadWithNonDefaultSectionName()
        {
            var config = MongoConfigurationSection.GetSection("mongoNonDefaultName");
            Assert.IsNotNull(config);
            Assert.AreEqual("Server=localhost:27018", config.Connections["local21018"].ConnectionString);
        }
    }
}