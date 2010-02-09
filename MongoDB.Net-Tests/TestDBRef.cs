

using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestDBRef
    {
        [Test]
        public void TestEqualsAreSameObject (){
            DBRef r = new DBRef ("tests", "2312314");
            Assert.AreEqual (r, r);
        }

        [Test]
        public void TestEqualsUsingSameValues (){
            String colname = "tests";
            String id = "32312312";
            DBRef r = new DBRef (colname, id);
            DBRef r2 = new DBRef (colname, id);
            
            Assert.AreEqual (r, r2);
        }

        [Test]
        public void TestFromDocument (){
            String colname = "tests";
            String id = "32312312";
            Document doc = new Document ().Append (DBRef.RefName, colname).Append (DBRef.IdName, id);
            
            DBRef expected = new DBRef (colname, id);
            Assert.AreEqual (expected, DBRef.FromDocument (doc));
        }

        [Test]
        public void TestFromIncompleteDocumentThrowsArguementException (){
            bool thrown = false;
            try {
                DBRef.FromDocument (new Document ().Append (DBRef.RefName, "tests"));
            } catch (ArgumentException) {
                thrown = true;
            }
            Assert.IsTrue (thrown, "ArgumentException should have been thrown when trying to create convert from incomplete document");
            
        }

        [Test]
        public void TestIsDocumentDBRef (){
            Document doc = new Document ();
            
            Assert.IsFalse (DBRef.IsDocumentDBRef (null));
            Assert.IsFalse (DBRef.IsDocumentDBRef (doc));
            
            doc[DBRef.RefName] = "tests";
            Assert.IsFalse (DBRef.IsDocumentDBRef (doc));
            
            doc.Remove (DBRef.RefName);
            doc[DBRef.IdName] = "12312131";
            Assert.IsFalse (DBRef.IsDocumentDBRef (doc));
            
            doc[DBRef.RefName] = "tests";
            Assert.IsTrue (DBRef.IsDocumentDBRef (doc));
            
            doc[DBRef.MetaName] = new Document();
            Assert.IsTrue (DBRef.IsDocumentDBRef (doc));
        }

        [Test]
        public void TestCastsToDocument (){
            OidGenerator ogen = new OidGenerator ();
            DBRef dref = new DBRef ("tests.dbrefs", ogen.Generate ());
            Document doc = (Document)dref;
            Assert.AreEqual (dref.CollectionName, doc[DBRef.RefName]);
        }
    }
}
