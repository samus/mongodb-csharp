using NUnit.Framework;

namespace MongoDB.UnitTests.Serialization.Descriptors
{
    [TestFixture]
    public class DocumentPropertyTests : SerializationTestBase
    {
        public class DocumentProperty
        {
            public Document A { get; set; }
        }

        [Test]
        public void CanSerialize()
        {
            var bson = Serialize<DocumentProperty>(new DocumentProperty() { A = new Document("B", "b") });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }

        [Test]
        public void CanSerializeUsingAnonymousType()
        {
            var bson = Serialize<DocumentProperty>(new { A = new { B = "b" } });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }
    }
}
