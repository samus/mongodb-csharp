using NUnit.Framework;

namespace MongoDB.Driver.Serialization.Builders
{
    [TestFixture]
    public class DocumentPropertyTests : SerializationTestBase
    {
        public class DocumentProperty
        {
            public Document A { get; set; }
        }

        [Test]
        public void CanDeserializeADocumentProperty()
        {
            //{ A: { B: "b" } }
            const string bson = "FgAAAANBAA4AAAACQgACAAAAYgAAAA==";
            var documentProperty = Deserialize<DocumentProperty>(bson);
            Assert.IsNotNull(documentProperty);
            Assert.IsNotNull(documentProperty.A);
            Assert.AreEqual("b", documentProperty.A["B"]);
        }
    }
}
