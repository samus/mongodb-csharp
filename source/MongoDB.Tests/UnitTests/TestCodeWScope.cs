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
        public void CanBeConstructedWithValue()
        {
            var code = new CodeWScope("code");

            Assert.AreEqual("code", code.Value);
        }

        [Test]
        public void CanBeConstructoredWithNull()
        {
            var code = new CodeWScope(null);

            Assert.IsNull(code.Value);
        }

        [Test]
        public void CanBeEqual()
        {
            var code1 = new CodeWScope("code", new Document("key", "value"));
            var code2 = new CodeWScope("code", new Document("key", "value"));

            Assert.AreEqual(code1, code2);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new CodeWScope("code",new Document("key","value"));
            var serializer = new XmlSerializer(typeof(CodeWScope));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (CodeWScope)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerializedWithNullValue()
        {
            var source = new CodeWScope(null,null);
            var serializer = new XmlSerializer(typeof(CodeWScope));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (CodeWScope)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }
    }
}