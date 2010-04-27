using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.UnitTests.Serialization.Builders
{
    [TestFixture]
    public class GenericListPropertyTests : SerializationTestBase
    {
        public class GenericListOfObjects
        {
            public List<object> A { get; set; }
        }

        [Test]
        public void CanDeserializeGenericListOfObjects()
        {
            //{ A: [1, 2] }
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<GenericListOfObjects>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            Assert.AreEqual(2, simpleArray.A.Count);
            Assert.Contains(1, simpleArray.A);
            Assert.Contains(2, simpleArray.A);
        }

        public class GenericListOfIntegers
        {
            public List<int> A { get; set; }
        }

        [Test]
        public void CanDeserializeGenericListOfIntegers()
        {
            //{ A: [1, 2] }
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<GenericListOfIntegers>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            Assert.AreEqual(2, simpleArray.A.Count);
            Assert.Contains(1, simpleArray.A);
            Assert.Contains(2, simpleArray.A);
        }

        public class GenericListOfClasses
        {
            public IList<GenericListOfClassesA> A { get; set; }
        }

        public class GenericListOfClassesA
        {
            public string B { get; set; }
        }

        [Test]
        public void CanDeserializeAGenericListOfClasses()
        {
            //{ A: [{ B: "b" }] }
            const string bson = "HgAAAARBABYAAAADMAAOAAAAAkIAAgAAAGIAAAAA";
            var objectArray = Deserialize<GenericListOfClasses>(bson);
            Assert.IsNotNull(objectArray);
            Assert.IsNotNull(objectArray.A);
            Assert.AreEqual(1, objectArray.A.Count);
            Assert.IsNotNull(objectArray.A[0].B);
            Assert.AreEqual("b", objectArray.A[0].B);
        }

        public class GenericListOfEmbeddedDocuments
        {
            public IList<Document> A { get; set; }
        }

        [Test]
        public void CanDeserializeAListOfEmbeddedDocuments()
        {
            //{ A: [{ B: "b" }] }
            const string bson = "HgAAAARBABYAAAADMAAOAAAAAkIAAgAAAGIAAAAA";
            var objectArray = Deserialize<GenericListOfEmbeddedDocuments>(bson);
            Assert.IsNotNull(objectArray);
            Assert.IsNotNull(objectArray.A);
            Assert.AreEqual(1, objectArray.A.Count);
            Assert.IsNotNull(objectArray.A[0]["B"]);
            Assert.AreEqual("b", objectArray.A[0]["B"]);
        }
    }
}