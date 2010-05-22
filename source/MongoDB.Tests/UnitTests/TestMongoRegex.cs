using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestMongoRegex
    {
        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new MongoRegex("exp", "opt");
            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (MongoRegex)formatter.Deserialize(mem);

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new MongoRegex("exp", "opt");
            var serializer = new XmlSerializer(typeof(Oid));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (MongoRegex)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }
    }
}