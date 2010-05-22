using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestDocument
    {
        private static void AreEqual(Document d1, Document d2)
        {
            if(!d1.Equals(d2))
                Assert.Fail(string.Format("Documents don't match\r\nExpected: {0}\r\nActual:   {1}", d1, d2));
        }

        private static void AreNotEqual(Document d1, Document d2)
        {
            if(d1.Equals(d2))
                Assert.Fail(string.Format("Documents match\r\nExpected: not {0}\r\nActual:       {1}", d1, d2));
        }

        private class ReverseComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return y.CompareTo(x);
            }
        }

        [Test]
        public void TestClearRemovesAll()
        {
            var d = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            Assert.AreEqual(3, d.Count);
            d.Clear();
            Assert.AreEqual(0, d.Count);
            Assert.IsNull(d["one"]);
            Assert.IsFalse(d.Contains("one"));
        }

        [Test]
        public void TestCopyToCopiesAndOverwritesKeys()
        {
            var d = new Document();
            var dest = new Document();
            dest["two"] = 200;
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            d.CopyTo(dest);
            Assert.AreEqual(2, dest["two"]);
        }

        [Test]
        public void TestCopyToCopiesAndPreservesKeyOrderToEmptyDoc()
        {
            var d = new Document();
            var dest = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            d.CopyTo(dest);
            var cnt = 1;
            foreach(var key in dest.Keys)
            {
                Assert.AreEqual(cnt, d[key]);
                cnt++;
            }
        }

        [Test]
        public void TestDocumentCanCreatedFromDictionary()
        {
            var dictionary = new Dictionary<string, object> {{"value1", "test"}, {"value2", 10}};
            var document = new Document(dictionary);
            Assert.AreEqual(2, document.Count);
            Assert.AreEqual("test", document["value1"]);
            Assert.AreEqual(10, document["value2"]);
        }

        [Test]
        public void TestDocumentIsSerializable()
        {
            var src = new Document().Add("test", 2);
            using(var mem = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(mem, src);
                mem.Position = 0;
                var dest = (Document)formatter.Deserialize(mem);
                AreEqual(src, dest);
            }
        }

        [Test]
        public void TestIdReturnsNullIfNotSet()
        {
            var document = new Document();
            Assert.IsNull(document.Id);
        }

        [Test]
        public void TestIdSets_IdField()
        {
            var document = new Document {Id = 10};
            Assert.AreEqual(10, document.Id);
        }

        [Test]
        public void TestInsertMaintainsKeyOrder()
        {
            var d = new Document();
            d["one"] = 1;
            d.Insert("zero", 0, 0);

            var keysList = d.Keys as IEnumerable<string>;
            foreach(var k in d.Keys)
            {
                Assert.AreEqual("zero", k);
                break;
            }
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(ArgumentException),
            MatchType = MessageMatch.Contains)]
        public void TestInsertWillThrowArgumentExceptionIfKeyAlreadyExists()
        {
            var d = new Document();
            d["one"] = 1;
            d.Insert("one", 1, 0);
        }

        [Test]
        public void TestKeyOrderIsPreserved()
        {
            var d = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            var cnt = 1;
            foreach(var key in d.Keys)
            {
                Assert.AreEqual(cnt, d[key]);
                cnt++;
            }
        }

        [Test]
        public void TestKeyOrderPreservedOnRemove()
        {
            var d = new Document();
            d["one"] = 1;
            d["onepointfive"] = 1.5;
            d.Add("two", 2);
            d.Add("two.5", 2.5);
            d.Remove("two.5");
            d["three"] = 3;
            d.Remove("onepointfive");
            var cnt = 1;
            foreach(var key in d.Keys)
            {
                Assert.AreEqual(cnt, d[key]);
                cnt++;
            }
        }

        [Test]
        public void TestMaintainsOrderUsingMultipleMethods()
        {
            var d = new Document(new ReverseComparer());
            d["one"] = 1;
            var test = d["one"];
            d["zero"] = 0;

            var keysList = d.Keys as IEnumerable<string>;
            Assert.AreEqual(keysList.First(), "zero");
        }

        [Test]
        public void TestRemove()
        {
            var d = new Document();
            d["one"] = 1;
            d.Remove("one");
            Assert.IsFalse(d.Contains("one"));
        }

        [Test]
        public void TestSetNullValue()
        {
            var document = new Document();
            document.Add("value", null);
            Assert.AreEqual(1, document.Count);
            Assert.IsNull(document["value"]);
        }

        [Test]
        public void TestTwoDocumentsWithDifferentDocumentChildTreeAreNotEqual()
        {
            var d1 = new Document().Add("k1", new Document().Add("k2", new Document().Add("k3", "foo")));
            var d2 = new Document().Add("k1", new Document().Add("k2", new Document().Add("k3", "bar")));
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithMisorderedArrayContentAreNotEqual()
        {
            var d1 = new Document().Add("k1", new[] {"v1", "v2"});
            var d2 = new Document().Add("k1", new[] {"v2", "v1"});
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameArrayContentAreEqual()
        {
            var d1 = new Document().Add("k1", new[] {"v1", "v2"});
            var d2 = new Document().Add("k1", new[] {"v1", "v2"});
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameContentInDifferentOrderAreNotEqual()
        {
            var d1 = new Document().Add("k1", "v1").Add("k2", "v2");
            var d2 = new Document().Add("k2", "v2").Add("k1", "v1");
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameContentInSameOrderAreEqual()
        {
            var d1 = new Document().Add("k1", "v1").Add("k2", "v2");
            var d2 = new Document().Add("k1", "v1").Add("k2", "v2");
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameDocumentChildTreeAreEqual()
        {
            var d1 = new Document().Add("k1", new Document().Add("k2", new Document().Add("k3", "foo")));
            var d2 = new Document().Add("k1", new Document().Add("k2", new Document().Add("k3", "foo")));
            AreEqual(d1, d2);
        }

        [Test]
        public void TestUseOfIComparerForKeys()
        {
            var doc = new Document(new ReverseComparer());

            doc.Append("a", 3);
            doc.Append("b", 2);
            doc.Append("c", 1);

            Assert.AreEqual("c", doc.Keys.First());
        }

        [Test]
        public void TestValues()
        {
            var d = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            var vals = d.Values;
            Assert.AreEqual(3, vals.Count);
        }

        [Test]
        public void TestValuesAdded()
        {
            var d = new Document();
            d["test"] = 1;
            Assert.AreEqual(1, d["test"]);
        }

        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new Document("key1", "value1").Add("key2", 10);
            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (Document)formatter.Deserialize(mem);

            Assert.AreEqual(2,dest.Count);
            Assert.AreEqual(source["key1"], dest["key1"]);
            Assert.AreEqual(source["key2"], dest["key2"]);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new Document("key1", "value1").Add("key2", new Document("key", "value").Add("key2", null));
            var serializer = new XmlSerializer(typeof(Document));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (Document)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(2, dest.Count);
            Assert.AreEqual(source["key1"], dest["key1"]);
            Assert.AreEqual(source["key2"], dest["key2"]);
        }
    }
}