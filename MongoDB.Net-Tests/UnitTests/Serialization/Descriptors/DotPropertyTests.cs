using NUnit.Framework;
using System.Collections.Generic;
using MongoDB.Driver.Attributes;

namespace MongoDB.Driver.UnitTests.Serialization.Descriptors
{
    [TestFixture]
    public class DotPropertyTests : SerializationTestBase
    {
        public class DotClass
        {
            [MongoAlias("a")]
            public List<DotChildA> A { get; set; }

            [MongoAlias("c")]
            public DotChildC C { get; set; }
        }

        public class DotChildA
        {
            [MongoAlias("b")]
            public int B { get; set; }
        }

        public class DotChildC
        {
            [MongoAlias("d")]
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