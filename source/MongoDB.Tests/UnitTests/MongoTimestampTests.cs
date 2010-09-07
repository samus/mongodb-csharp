using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    public class MongoTimestampTests
    {
        [Test]
        public void Ctor_WithoutArgs_CanBeCreated()
        {
            var stamp = new MongoTimestamp();

            Assert.AreEqual(0, stamp.Value);
        }

        [Test]
        public void Ctor_WithLong_CanBeCreatedAndValueIsLongValue()
        {
            const long value = long.MaxValue - 100;
            var stmap = new MongoTimestamp(value);

            Assert.AreEqual(value, stmap.Value);
        }

        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new MongoTimestamp(long.MaxValue - 100);

            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (MongoTimestamp)formatter.Deserialize(mem);

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new MongoTimestamp(long.MaxValue - 100);
            var serializer = new XmlSerializer(typeof(MongoTimestamp));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (MongoTimestamp)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }
    }
}