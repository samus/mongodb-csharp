using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestCode
    {
        [Test]
        public void CanBeConstructedWithValue()
        {
            var code = new Code("code");

            Assert.AreEqual("code",code.Value);
        }

        [Test]
        public void CanBeConstructoredWithNull()
        {
            var code = new Code(null);

            Assert.IsNull(code.Value);
        }

        [Test]
        public void CanBeEqual()
        {
            var code1 = new Code("code");
            var code2 = new Code("code");
            
            Assert.AreEqual(code1,code2);
        }

        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new Code("code");
            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (Code)formatter.Deserialize(mem);

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new Code("code");
            var serializer = new XmlSerializer(typeof(Code));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (Code)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerializedWithNullValue()
        {
            var source = new Code(null);
            var serializer = new XmlSerializer(typeof(Code));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (Code)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }
    }
}