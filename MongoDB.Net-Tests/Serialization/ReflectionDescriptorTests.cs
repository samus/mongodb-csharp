using NUnit.Framework;

namespace MongoDB.Driver.Serialization
{
    [TestFixture]
    public class ReflectionDescriptorTests : SerializationTestBase
    {
        [Test]
        public void CanSerializeASimpleObject()
        {
            var bson = Serialize(new { A = "a", B = "b", C = new { D = "d" } });
            Assert.AreEqual("KAAAAAJBAAIAAABhAAJCAAIAAABiAANDAA4AAAACRAACAAAAZAAAAA==", bson);
        }

        [Test]
        public void CanSerializeAnSimpleArray()
        {
            var bson = Serialize(new { A = new[] { 1, 2 } });
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }

        [Test]
        public void CanSerializeAnObjectArray()
        {
            var bson = Serialize(new { A = new object[] { new { B = "b" }, new { C = "c" } } });
            Assert.AreEqual("LwAAAARBACcAAAADMAAOAAAAAkIAAgAAAGIAAAMxAA4AAAACQwACAAAAYwAAAAA=", bson);
        }

        [Test]
        public void CanSerializeAnDocumentPreperty()
        {
            var bson = Serialize(new { A = new Document().Append("B", "b") });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }

        [Test]
        public void CanSerializeAnDocument()
        {
            var bson = Serialize(new Document().Append("A", "a"));
            Assert.AreEqual("DgAAAAJBAAIAAABhAAA=", bson);
        }
    }
}