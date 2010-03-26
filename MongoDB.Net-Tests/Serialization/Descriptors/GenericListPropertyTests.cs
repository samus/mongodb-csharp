using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver.Serialization
{
    [TestFixture]
    public class GenericListPropertyTests : SerializationTestBase
    {
        public class GenericListOfObjectsProperty
        {
            public List<object> A { get; set; }
        }

        [Test]
        public void CanSerializeAGenericListOfObjects()
        {
            var bson = Serialize<GenericListOfObjectsProperty>(new GenericListOfObjectsProperty() { A = new List<object> { 1, 2 } });
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }

        [Test]
        public void CanSerializeAGenericListOfObjectsUsingAnonymousType()
        {
            var bson = Serialize<GenericListOfObjectsProperty>(new { A = new[] { 1, 2 } });
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }

        public class GenericListOfIntegerProperty
        {
            public List<int> A { get; set; }
        }

        [Test]
        public void CanSerializeAGenericListOfIntegers()
        {
            var bson = Serialize<GenericListOfIntegerProperty>(new GenericListOfIntegerProperty() { A = new List<int> { 1, 2 } });
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
        }

        [Test]
        public void CanSerializeAGenericListOfIntegersUsingAnonymousType()
        {
            var bson = Serialize<GenericListOfIntegerProperty>(new { A = new[] { 1, 2 } });
            Assert.AreEqual("GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA", bson);
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
        public void CanSerializeAGenericListOfClasses()
        {
            var o = new GenericListOfClasses();
            o.A = new List<GenericListOfClassesA> { new GenericListOfClassesA() { B = "b" } };
            string bson = Serialize<GenericListOfClasses>(o);
            Assert.AreEqual("HgAAAARBABYAAAADMAAOAAAAAkIAAgAAAGIAAAAA", bson);
        }

        [Test]
        public void CanSerializeAGenericListOfClassesUsingAnonymousType()
        {
            string bson = Serialize<GenericListOfClasses>(new { A = new[] { new { B = "b" } } });
            Assert.AreEqual("HgAAAARBABYAAAADMAAOAAAAAkIAAgAAAGIAAAAA", bson);
        }

        public class GenericListOfEmbeddedDocuments
        {
            public IList<Document> A { get; set; }
        }

        [Test]
        public void CanSerializeAListOfEmbeddedDocuments()
        {
            var o = new GenericListOfEmbeddedDocuments();
            o.A = new List<Document> { new Document().Append("B", "b") };
            string bson = Serialize<GenericListOfEmbeddedDocuments>(o);
            Assert.AreEqual("HgAAAARBABYAAAADMAAOAAAAAkIAAgAAAGIAAAAA", bson);
        }

        [Test]
        public void CanSerializeAListOfEmbeddedDocumentsUsingAnonymousType()
        {
            string bson = Serialize<GenericListOfEmbeddedDocuments>(new { A = new[] { new Document("B", "b") } });
            Assert.AreEqual("HgAAAARBABYAAAADMAAOAAAAAkIAAgAAAGIAAAAA", bson);
        }
    }
}