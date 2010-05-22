using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestCodeWScope
    {
        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new CodeWScope("code", new Document("key", "value"));
            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (CodeWScope)formatter.Deserialize(mem);

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new CodeWScope("code", new Document("key", "value"));
            var serializer = new XmlSerializer(typeof(CodeWScope));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (CodeWScope)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }

    }
}