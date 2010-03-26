using NUnit.Framework;

namespace MongoDB.Driver.Serialization.Descriptors
{
    [TestFixture]
    public class EmbeddedClassPropertyTests : SerializationTestBase
    {
        public class SimpleObject
        {
            public string A { get; set; }
            public string B { get; set; }
            public SimpleObjectC C { get; set; }
        }
        public class SimpleObjectC
        {
            public string D { get; set; }
        }

        [Test]
        public void CanSerializeASimpleObject()
        {
            var bson = Serialize<SimpleObject>(new { A = "a", B = "b", C = new { D = "d" } });
            Assert.AreEqual("KAAAAAJBAAIAAABhAAJCAAIAAABiAANDAA4AAAACRAACAAAAZAAAAA==", bson);
        }
    }
}