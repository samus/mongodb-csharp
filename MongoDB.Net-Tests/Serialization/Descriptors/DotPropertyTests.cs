using NUnit.Framework;
using System.Collections.Generic;
using MongoDB.Driver.Attributes;

namespace MongoDB.Driver.Serialization.Descriptors
{
    [TestFixture]
    public class DotPropertyTests : SerializationTestBase
    {
        public class DotClass
        {
            [MongoName("a")]
            public List<DotChildA> A { get; set; }

            [MongoName("c")]
            public DotChildC C { get; set; }
        }

        public class DotChildA
        {
            [MongoName("b")]
            public int B { get; set; }
        }

        public class DotChildC
        {
            [MongoName("d")]
            public int D { get; set; }
        }

        [Test]
        public void CanSerializeWithChild()
        {
            var expected = Serialize(new Document("c.d", 10));
            var bson = Serialize<DotClass>(new Document("C.D", 10));
            Assert.AreEqual(expected, bson);
        }

        [Test]
        public void CanSerializeWithChildIndexer()
        {
            var expected = Serialize(new Document("a.5.b", 10));
            var bson = Serialize<DotClass>(new Document("A.5.B", 10));
            Assert.AreEqual(expected, bson);
        }
    }
}