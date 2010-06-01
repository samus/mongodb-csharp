using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestMongoRegex
    {
        [Test]
        public void CanBeCunstructedFromNullExpressionAndOptions()
        {
            var regex = new MongoRegex(null, null);
            Assert.IsNull(regex.Expression);
            Assert.IsNull(regex.RawOptions);
        }

        [Test]
        public void CanBeConstructed()
        {
            var regex = new MongoRegex("expression");
            Assert.AreEqual("expression", regex.Expression);
            Assert.AreEqual(string.Empty, regex.RawOptions);
        }

        [Test]
        public void CanBeConstructedWithOption()
        {
            var regex = new MongoRegex("expression", "options");
            Assert.AreEqual("expression",regex.Expression);
            Assert.AreEqual("options",regex.RawOptions);
        }

        [Test]
        public void CanBeConstructedFromRegex()
        {
            const RegexOptions options = RegexOptions.IgnoreCase |
                                         RegexOptions.IgnorePatternWhitespace |
                                         RegexOptions.Multiline;

            var regex = new MongoRegex(new Regex("expression", options));
            Assert.AreEqual("expression", regex.Expression);
            Assert.AreEqual("img", regex.RawOptions);
        }

        [Test]
        public void CanBeConstructedWithMongoRegexOption()
        {
            var regex = new MongoRegex("expression", MongoRegexOption.IgnoreCase | MongoRegexOption.IgnorePatternWhitespace | MongoRegexOption.Multiline);
            Assert.AreEqual("expression", regex.Expression);
            Assert.AreEqual("img", regex.RawOptions);
        }

        [Test]
        public void CanReadOptions()
        {
            var regex = new MongoRegex("expression", "img");
            Assert.AreEqual(MongoRegexOption.IgnoreCase | MongoRegexOption.IgnorePatternWhitespace | MongoRegexOption.Multiline, regex.Options);
        }

        [Test]
        public void CanSetOptions()
        {
            var regex = new MongoRegex("expression", null)
            {
                Options = MongoRegexOption.IgnoreCase & MongoRegexOption.IgnorePatternWhitespace
            };

            Assert.AreEqual("ig",regex.RawOptions);
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