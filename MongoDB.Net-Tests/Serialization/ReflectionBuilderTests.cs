using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver.Serialization
{
    [TestFixture]
    public class ReflectionBuilderTests : SerializationTestBase
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
            const string bson = "KAAAAAJBAAIAAABhAAJCAAIAAABiAANDAA4AAAACRAACAAAAZAAAAA==";
            var simpleObject = Deserialize<SimpleObject>(bson);
            Assert.IsNotNull(simpleObject);
            Assert.AreEqual("a", simpleObject.A);
            Assert.AreEqual("b", simpleObject.B);
            Assert.IsNotNull(simpleObject.C);
            Assert.AreEqual("d", simpleObject.C.D);
        }

        public class SimpleArrayAsList
        {
            public List<object> A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsList()
        {
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<SimpleArrayAsList>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            Assert.AreEqual(2, simpleArray.A.Count);
            Assert.Contains(1, simpleArray.A);
            Assert.Contains(2, simpleArray.A);
        }

        public class SimpleArrayAsListOfInt
        {
            public List<int> A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsListOfInt()
        {
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<SimpleArrayAsListOfInt>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            Assert.AreEqual(2, simpleArray.A.Count);
            Assert.Contains(1, simpleArray.A);
            Assert.Contains(2, simpleArray.A);
        }

        public class SimpleArrayAsArrayList
        {
            public ArrayList A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsArrayList()
        {
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<SimpleArrayAsArrayList>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            Assert.AreEqual(2, simpleArray.A.Count);
            Assert.Contains(1, simpleArray.A);
            Assert.Contains(2, simpleArray.A);
        }

        public class SimpleArrayAsIEnumerable
        {
            public IEnumerable A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsIEnumerable()
        {
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<SimpleArrayAsIEnumerable>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            var list = new List<object>();
            foreach(var value in simpleArray.A)
                list.Add(value);
            Assert.AreEqual(2, list.Count);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
        }

        public class SimpleArrayAsIEnumerableOfInt
        {
            public IEnumerable<int> A { get; set; }
        }

        [Test]
        public void CanDeserializeAnSimpleArrayAsIEnumerableOfInt()
        {
            const string bson = "GwAAAARBABMAAAAQMAABAAAAEDEAAgAAAAAA";
            var simpleArray = Deserialize<SimpleArrayAsIEnumerableOfInt>(bson);
            Assert.IsNotNull(simpleArray);
            Assert.IsNotNull(simpleArray.A);
            var list = new List<int>(simpleArray.A);
            Assert.AreEqual(2, list.Count);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
        }

        public class ObjectArray
        {
            public IList<ObjectArrayA> A { get; set; }
        }

        public class ObjectArrayA
        {
            public string B { get; set; }
        }

        [Test]
        public void CanDeserializeAnObjectArray()
        {
            const string bson = "HgAAAARBABYAAAADMAAOAAAAAkIAAgAAAGIAAAAA";
            var objectArray = Deserialize<ObjectArray>(bson);
            Assert.IsNotNull(objectArray);
            Assert.IsNotNull(objectArray.A);
            Assert.AreEqual(1, objectArray.A.Count);
            Assert.IsNotNull(objectArray.A[0].B);
            Assert.AreEqual("b", objectArray.A[0].B);
        }

        public class DocumentProperty
        {
            public Document A { get; set; }
        }

        [Test]
        public void CanDeserializeAnDocumentPreperty()
        {
            const string bson = "FgAAAANBAA4AAAACQgACAAAAYgAAAA==";
            var documentProperty = Deserialize<DocumentProperty>(bson);
            Assert.IsNotNull(documentProperty);
            Assert.IsNotNull(documentProperty.A);
            Assert.AreEqual("b", documentProperty.A["B"]);
        }

        [Test]
        public void CanSerializeAnDocument()
        {
            const string bson = "DgAAAAJBAAIAAABhAAA=";
            var document = Deserialize<Document>(bson);
            Assert.IsNotNull(document);
            Assert.AreEqual(1, document.Count);
            Assert.AreEqual("a", document["A"]);
        }
    }
}