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
        public void CanBeCunstructedFromNullExceptionAndOptions()
        {
            var regex = new MongoRegex(null, null);
            Assert.IsNull(regex.Expression);
            Assert.IsNull(regex.Options);
        }

        [Test]
        public void CanBeConstructed()
        {
            var regex = new MongoRegex("expression", "options");
            Assert.AreEqual("expression",regex.Expression);
            Assert.AreEqual("options",regex.Options);
        }

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
            var serializer = new XmlSerializer(typeof(MongoRegex));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (MongoRegex)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerializedWhenNullPropertys()
        {
            var source = new MongoRegex(null, null);
            var serializer = new XmlSerializer(typeof(MongoRegex));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (MongoRegex)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }
    }
}