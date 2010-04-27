using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver.UnitTests.Serialization.Descriptors
{
    [TestFixture]
    public class ExtendedPropertiesTests : SerializationTestBase
    {
        public class IDictionaryProperty
        {
            public IDictionary<string, object> ExtendedProperties { get; set; }
        }

        [Test]
        public void CanSerializeUsingIDictionary()
        {
            var bson = Serialize<IDictionaryProperty>(new IDictionaryProperty() { ExtendedProperties = new Dictionary<string, object> { { "A", new Document("B", "b") } } });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }

        public class DictionaryProperty
        {
            public Dictionary<string, object> ExtendedProperties { get; set; }
        }

        [Test]
        public void CanSerializeUsingDictionary()
        {
            var bson = Serialize<DictionaryProperty>(new DictionaryProperty() { ExtendedProperties = new Dictionary<string, object> { { "A", new Document("B", "b") } } });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }

        public class DocumentProperty
        {
            public Document ExtendedProperties { get; set; }
        }

        [Test]
        public void CanSerializeUsingDocument()
        {
            var bson = Serialize<DocumentProperty>(new DocumentProperty() { ExtendedProperties = new Document("A", new Document("B", "b")) });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }
    }
}