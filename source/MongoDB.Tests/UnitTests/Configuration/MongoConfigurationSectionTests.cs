using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using MongoDB.Configuration;
using NUnit.Framework;

namespace MongoDB.UnitTests.Configuration
{
    [TestFixture]
    public class MongoConfigurationSectionTests
    {
        private MongoConfigurationSection ReadFromFile(int index)
        {
            var name = string.Format("{0}.{1}.config", GetType().FullName, index);
            var assembly = Assembly.GetExecutingAssembly();
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                using(var stream = assembly.GetManifestResourceStream(name))
                using(var reader = new StreamReader(stream, Encoding.Default, false))
                    File.WriteAllText(tmpFile.FullName, reader.ReadToEnd(),Encoding.Default);

                var map = new ExeConfigurationFileMap { ExeConfigFilename = tmpFile.FullName };
                var exeConfiguration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                return exeConfiguration.GetSection(MongoConfigurationSection.DefaultSectionName) as MongoConfigurationSection;
            }
            finally
            {
                if(tmpFile.Exists)   
                    tmpFile.Delete();
            }
        }

        [Test]
        public void CanReadFromTestsConfig()
        {
            var section = MongoConfigurationSection.GetSection();
            Assert.IsNotNull(section);
            Assert.AreEqual("Server=localhost:27017", section.Connections["default"].ConnectionString);
            Assert.AreEqual("Server=localhost:27018", section.Connections["local21018"].ConnectionString);
        }

        [Test]
        public void CanReadWithNonDefaultSectionName()
        {
            var section = MongoConfigurationSection.GetSection("mongoNonDefaultName");
            Assert.IsNotNull(section);
            Assert.AreEqual("Server=localhost:27018", section.Connections["local21018"].ConnectionString);
        }

        [Test]
        public void CanCreateConfigurationFromSection()
        {
            var section = MongoConfigurationSection.GetSection();
            var config = section.CreateConfiguration();
            Assert.IsNotNull(config);
            Assert.AreEqual("Server=localhost:27017", config.ConnectionString);
        }

        [Test]
        public void CanUpdateConfigurationFromSection()
        {
            var section = ReadFromFile(1);
            var config = new MongoConfiguration();
            Assert.IsEmpty(config.ConnectionString);
            section.UpdateConfiguration(config);
            Assert.AreEqual("Server=localhost:27017", config.ConnectionString);
        }
    }
}