using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;

namespace MongoDB.UnitTests
{
    [TestFixture]
    public class TestDBRef
    {
        [Test]
        public void CanBeBinarySerialized()
        {
            var source = new DBRef("collection", "id");
            var formatter = new BinaryFormatter();

            var mem = new MemoryStream();
            formatter.Serialize(mem, source);
            mem.Position = 0;

            var dest = (DBRef)formatter.Deserialize(mem);

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void CanBeXmlSerialized()
        {
            var source = new DBRef("collection", "id");
            var serializer = new XmlSerializer(typeof(Oid));

            var writer = new StringWriter();
            serializer.Serialize(writer, source);
            var dest = (DBRef)serializer.Deserialize(new StringReader(writer.ToString()));

            Assert.AreEqual(source, dest);
        }

        [Test]
        public void TestCastsToDocument()
        {
            var ogen = new OidGenerator();
            var dref = new DBRef("tests.dbrefs", ogen.Generate());
            var doc = (Document)dref;
            Assert.AreEqual(dref.CollectionName, doc[DBRef.RefName]);
        }

        [Test]
        public void TestEqualsAreSameObject()
        {
            var r = new DBRef("tests", "2312314");
            Assert.AreEqual(r, r);
        }

        [Test]
        public void TestEqualsUsingSameValues()
        {
            const string colname = "tests";
            const string id = "32312312";
            var r = new DBRef(colname, id);
            var r2 = new DBRef(colname, id);

            Assert.AreEqual(r, r2);
        }

        [Test]
        public void TestFromDocument()
        {
            const string colname = "tests";
            const string id = "32312312";
            var doc = new Document().Add(DBRef.RefName, colname).Add(DBRef.IdName, id);

            var expected = new DBRef(colname, id);
            Assert.AreEqual(expected, DBRef.FromDocument(doc));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestFromIncompleteDocumentThrowsArguementException()
        {
            DBRef.FromDocument(new Document(DBRef.RefName, "tests"));
        }

        [Test]
        public void TestIsDocumentDBRef()
        {
            var doc = new Document();

            Assert.IsFalse(DBRef.IsDocumentDBRef(null));
            Assert.IsFalse(DBRef.IsDocumentDBRef(doc));

            doc[DBRef.RefName] = "tests";
            Assert.IsFalse(DBRef.IsDocumentDBRef(doc));

            doc.Remove(DBRef.RefName);
            doc[DBRef.IdName] = "12312131";
            Assert.IsFalse(DBRef.IsDocumentDBRef(doc));

            doc[DBRef.RefName] = "tests";
            Assert.IsTrue(DBRef.IsDocumentDBRef(doc));

            doc[DBRef.MetaName] = new Document();
            Assert.IsTrue(DBRef.IsDocumentDBRef(doc));
        }
    }
}