using NUnit.Framework;

namespace MongoDB.UnitTests.Serialization.Descriptors
{
    [TestFixture]
    [Ignore("Currently we do not plan to support this. As an alternative you can use Mo or Linq.")]
    public class ModifierTests : SerializationTestBase
    {
        public class ModifierEntity
        {
            public int A { get; set; }
        }

        [Test]
        public void CanSerializeIncrementUsingAnonymousType()
        {
            var expectedBson = Serialize(new Document("$inc", new Document("A", 1)));
            var bson = Serialize<ModifierEntity>(new { A = new Document("$inc", 1) });

            Assert.AreEqual(expectedBson, bson);
        }

        [Test]
        public void CanSerializeSetUsingAnonymousType()
        {
            var expectedBson = Serialize(new Document("$set", new Document("A", 4)));
            var bson = Serialize<ModifierEntity>(new { A = new Document("$inc", 1) });

            Assert.AreEqual(expectedBson, bson);
        }
    }
}