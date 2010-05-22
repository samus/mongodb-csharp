using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestMongoServerEndPoint
    {
        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new MongoServerEndPoint("myserver", 12345);
            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (MongoServerEndPoint)formatter.Deserialize(mem);

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new MongoServerEndPoint("myserver", 12345);
            var serializer = new XmlSerializer(typeof(MongoServerEndPoint));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (MongoServerEndPoint)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }
    }
}