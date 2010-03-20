using MongoDB.Driver.Serialization;
using MongoDB.Driver.Serialization.Attributes;
using NUnit.Framework;

namespace MongoDB.Driver.Tests.Serialization
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
            var bson = Serialize(new { A = new Document().Add("B", "b") });
            Assert.AreEqual("FgAAAANBAA4AAAACQgACAAAAYgAAAA==", bson);
        }

        [Test]
        public void CanSerializeAnDocument()
        {
            var bson = Serialize(new Document().Add("A", "a"));
            Assert.AreEqual("DgAAAAJBAAIAAABhAAA=", bson);
        }

        public class TestPersonUpdateWithAnonymusClass
        {
            public class TestAddress
            {
                [MongoName("sn")]
                public string StreetName { get; set; }
                public int Number { get; set; }
            }

            public TestAddress Address { get; set; }
        }

        [Test]
        public void CanUpdateWithAnonymusClass(){
            var expectedBson = Serialize(new Document("Address",new Document("sn","test")));
            var bson = Serialize(new { Address = new { StreetName = "test" } }, typeof(TestPersonUpdateWithAnonymusClass));
            Assert.AreEqual(expectedBson,bson);
        }

        public class TestPersonChangeMongoFieldName
        {
            [MongoName("nm")]
            public string Name { get; set; }
        }

        [Test]
        public void CanChangeMongoFieldName(){
            var expectedBson = Serialize(new Document("nm","test"));
            var bson = Serialize(new TestPersonChangeMongoFieldName { Name = "test" }, typeof(TestPersonChangeMongoFieldName));
            Assert.AreEqual(expectedBson, bson);
        }
    }
}