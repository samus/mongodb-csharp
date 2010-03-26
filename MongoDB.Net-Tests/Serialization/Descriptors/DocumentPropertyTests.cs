using NUnit.Framework;

namespace MongoDB.Driver.Serialization.Descriptors
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
            var bson = Serialize<DocumentProperty>(new { A = new Document("B", "b") });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }
    }
}
