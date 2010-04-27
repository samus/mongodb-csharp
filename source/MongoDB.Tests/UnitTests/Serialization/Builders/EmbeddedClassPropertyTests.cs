using NUnit.Framework;

namespace MongoDB.UnitTests.Serialization.Builders
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
        public void CanDeserializeASimpleObject()
        {
            //{ A: "a", B: "b", C: { D: "d" } }
            const string bson = "KAAAAAJBAAIAAABhAAJCAAIAAABiAANDAA4AAAACRAACAAAAZAAAAA==";
            var simpleObject = Deserialize<SimpleObject>(bson);
            Assert.IsNotNull(simpleObject);
            Assert.AreEqual("a", simpleObject.A);
            Assert.AreEqual("b", simpleObject.B);
            Assert.IsNotNull(simpleObject.C);
            Assert.AreEqual("d", simpleObject.C.D);
        }
    }
}
