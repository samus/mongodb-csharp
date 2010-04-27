using NUnit.Framework;

namespace MongoDB.Driver.Serialization.Builders
{
    [TestFixture]
    public class ValueConversionTests : SerializationTestBase
    {
        public class SimpleObject
        {
            public bool A { get; set; }
        }

        [Test]
        public void CanConvertSimpleValues(){
            var bson = Serialize(new Document("A", 1.0));
            var result = Deserialize<SimpleObject>(bson);

            Assert.IsTrue(result.A);
        }
    }
}