using NUnit.Framework;
using MongoDB.Driver.Attributes;

namespace MongoDB.Driver.UnitTests.Serialization.Descriptors
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
            var bson = Serialize<SimpleObject>(new SimpleObject() { A = "a", B = "b", C = new SimpleObjectC() { D = "d" } });
            Assert.AreEqual("KAAAAAJBAAIAAABhAAJCAAIAAABiAANDAA4AAAACRAACAAAAZAAAAA==", bson);
        }

        [Test]
        public void CanSerializeASimpleObjectUsingAnAnonymousType()
        {
            var bson = Serialize<SimpleObject>(new { A = "a", B = "b", C = new { D = "d" } });
            Assert.AreEqual("KAAAAAJBAAIAAABhAAJCAAIAAABiAANDAA4AAAACRAACAAAAZAAAAA==", bson);
        }

        [Test]
        public void CanSerializeASimpleObjectWithANullProperty()
        {
            var bson = Serialize<SimpleObject>(new SimpleObject());
            Assert.AreEqual("DgAAAApBAApCAApDAAA=", bson);
        }

        public class SuperClass
        {
            [MongoAlias("a")]
            public SuperClassA A { get; set; }
        }

        public class SuperClassA
        {
            [MongoAlias("b")]
            public string B { get; set; }
        }

        [Test]
        public void CanSerializeAnEmbeddedClassPropertyUsingDotSyntaxWhenAliasesExist()
        {
            var expected = Serialize(new Document("a.b", "b"));
            var bson = Serialize<SuperClass>(new Document("A.B", "b"));
            Assert.AreEqual(expected, bson);
        }
    }
}