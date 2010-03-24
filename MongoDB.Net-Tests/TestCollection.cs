using System;
using System.Linq;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollection : MongoTestBase
    {
        private string pound = "\u00a3";

        public override string TestCollections
        {
            get { return "inserts,updates,counts,counts_spec,finds,charreads,saves"; }
        }

        public override void OnInit()
        {
            var finds = DB["finds"];
            for(var j = 1; j < 100; j++)
                finds.Insert(new Document {{"x", 4}, {"h", "hi"}, {"j", j}});
            for(var j = 100; j < 105; j++)
                finds.Insert(new Document {{"x", 4}, {"n", 1}, {"j", j}});
            var charreads = DB["charreads"];
            charreads.Insert(new Document {{"test", "1234" + pound + "56"}});
        }

        private int CountDocs(ICursor cur)
        {
            var cnt = 0;
            foreach(var doc in cur.Documents)
                cnt++;
            return cnt;
        }

        [Test]
        public void TestArrayInsert()
        {
            var inserts = DB["inserts"];
            var indoc1 = new Document();
            indoc1["song"] = "The Axe";
            indoc1["artist"] = "Tinsley Ellis";
            indoc1["year"] = 2006;

            var indoc2 = new Document();
            indoc2["song"] = "The Axe2";
            indoc2["artist"] = "Tinsley Ellis2";
            indoc2["year"] = 2008;

            inserts.Insert(new[] {indoc1, indoc2});

            var result = inserts.FindOne(new Document().Add("song", "The Axe"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2006, result["year"]);

            result = inserts.FindOne(new Document().Add("song", "The Axe2"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2008, result["year"]);
        }

        [Test]
        public void TestCount()
        {
            var counts = DB["counts"];
            var top = 100;
            for(var i = 0; i < top; i++)
                counts.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam").Add("cnt", i));
            var cnt = counts.Count();
            Assert.AreEqual(top, cnt, "Count not the same as number of inserted records");
        }

        [Test]
        public void TestCountInvalidCollection()
        {
            var counts = DB["counts_wtf"];
            Assert.AreEqual(0, counts.Count());
        }

        [Test]
        public void TestCountWithSpec()
        {
            var counts = DB["counts_spec"];
            counts.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam").Add("cnt", 1));
            counts.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam").Add("cnt", 2));
            counts.Insert(new Document().Add("Last", "Corder").Add("First", "Sam").Add("cnt", 3));

            Assert.AreEqual(2, counts.Count(new Document().Add("Last", "Cordr")));
            Assert.AreEqual(1, counts.Count(new Document().Add("Last", "Corder")));
            Assert.AreEqual(0, counts.Count(new Document().Add("Last", "Brown")));
        }

        [Test]
        public void TestDelete()
        {
            var deletes = DB["deletes"];
            var doc = new Document();
            doc["y"] = 1;
            doc["x"] = 2;
            deletes.Insert(doc);

            var selector = new Document().Add("x", 2);

            var result = deletes.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result["y"]);

            deletes.Delete(selector);
            result = deletes.FindOne(selector);
            Assert.IsNull(result, "Shouldn't have been able to find a document that was deleted");
        }

        [Test]
        public void TestFindAttributeLimit()
        {
            var query = new Document();
            query["j"] = 10;
            var fields = new Document();
            fields["x"] = 1;

            var c = DB["finds"].Find(query, -1, 0, fields);
            foreach(var result in c.Documents)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(4, result["x"]);
                Assert.IsNull(result["j"]);
            }
        }

        [Test]
        public void TestFindGTRange()
        {
            var query = new Document();
            query["j"] = new Document().Add("$gt", 20);

            var c = DB["finds"].Find(query);
            foreach(var result in c.Documents)
            {
                Assert.IsNotNull(result);
                var j = result["j"];
                Assert.IsTrue(Convert.ToDouble(j) > 20);
            }
        }

        [Test]
        public void TestFindNulls()
        {
            var query = new Document().Add("n", null);
            var numnulls = DB["finds"].Count(query);
            Assert.AreEqual(99, numnulls);
        }

        [Test]
        public void TestFindOne()
        {
            var query = new Document();
            query["j"] = 10;
            var result = DB["finds"].FindOne(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result["x"]);
            Assert.AreEqual(10, result["j"]);
        }

        [Test]
        public void TestFindOneNotThere()
        {
            var query = new Document();
            query["not_there"] = 10;
            var result = DB["finds"].FindOne(query);
            Assert.IsNull(result);
        }

        [Test]
        public void TestFindOneObjectContainingUKPound()
        {
            var query = new Document();
            var result = DB["charreads"].FindOne(query);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("test"));
            Assert.AreEqual("1234£56", result["test"]);
        }

        [Test]
        public void TestFindWhereEquivalency()
        {
            var col = DB["finds"];
            var lt = new Document().Add("j", new Document().Add("$lt", 5));
            var where = "this.j < 5";
            var explicitWhere = new Document().Add("$where", new Code(where));
            var func = new CodeWScope("function() { return this.j < 5; }", new Document());
            var funcDoc = new Document().Add("$where", func);

            Assert.AreEqual(4, CountDocs(col.Find(lt)), "Basic find didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(where)), "String where didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(explicitWhere)), "Explicit where didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(funcDoc)), "Function where didn't return 4 docs");
        }

        [Test]
        public void TestInsertBulkLargerThan4MBOfDocuments()
        {
            var b = new Binary(new byte[1024*1024*2]);
            var inserts = DB["inserts"];
            try
            {
                var docs = new Document[10];
                //6MB+ of documents
                for(var x = 0; x < docs.Length; x++)
                    docs[x] = new Document {{"name", "bulk"}, {"b", b}, {"x", x}};
                inserts.Insert(docs, true);
                var count = inserts.Count(new Document {{"name", "bulk"}});
                Assert.AreEqual(docs.Length, count, "Wrong number of documents inserted");
            }
            catch(MongoException)
            {
                Assert.Fail("MongoException should not have been thrown.");
            }
        }

        [Test]
        public void TestInsertLargerThan4MBDocument()
        {
            var b = new Binary(new byte[1024*1024]);
            var big = new Document {{"name", "Big Document"}, {"b1", b}, {"b2", b}, {"b3", b}, {"b4", b}};
            var inserts = DB["inserts"];
            var thrown = false;
            try
            {
                inserts.Insert(big);
            }
            catch(MongoException)
            {
                thrown = true;
            }
            catch(Exception e)
            {
                Assert.Fail("Wrong Exception thrown " + e.GetType().Name);
            }
            Assert.IsTrue(thrown, "Shouldn't be able to insert large document");
        }

        [Test]
        public void TestInsertOfArray()
        {
            var ogen = new OidGenerator();
            var inserts = DB["inserts"];
            var album = new Document();
            album["_id"] = ogen.Generate();
            album["artist"] = "Popa Chubby";
            album["title"] = "Deliveries After Dark";
            album["songs"] = new[]
            {
                new Document().Add("title", "Let The Music Set You Free").Add("length", "5:15").Add("_id", ogen.Generate()),
                new Document().Add("title", "Sally Likes to Run").Add("length", "4:06").Add("_id", ogen.Generate()),
                new Document().Add("title", "Deliveries After Dark").Add("length", "4:17").Add("_id", ogen.Generate()),
                new Document().Add("title", "Theme From The Godfather").Add("length", "3:06").Add("_id", ogen.Generate()),
                new Document().Add("title", "Grown Man Crying Blues").Add("length", "8:09").Add("_id", ogen.Generate()),
            };
            inserts.Insert(album);

            var result = inserts.FindOne(new Document().Add("songs.title", "Deliveries After Dark"));
            Assert.IsNotNull(result);

            Assert.AreEqual(album.ToString(), result.ToString());
        }

        [Test]
        public void TestManualWhere()
        {
            var query = new Document().Add("$where", new Code("this.j % 2 == 0"));
            var c = DB["finds"].Find(query);
            foreach(var result in c.Documents)
            {
                Assert.IsNotNull(result);
                var j = result["j"];
                Assert.IsTrue(Convert.ToInt32(j)%2 == 0);
            }
        }

        [Test]
        public void TestPoundSymbolInsert()
        {
            var inserts = DB["inserts"];
            var indoc = new Document().Add("x", "1234" + pound + "56").Add("y", 1);
            inserts.Insert(indoc);

            var result = inserts.FindOne(new Document().Add("x", "1234" + pound + "56"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result["y"]);
        }

        [Test]
        public void TestReallySimpleInsert()
        {
            var inserts = DB["inserts"];
            var indoc = new Document();
            indoc["y"] = 1;
            indoc["x"] = 2;
            inserts.Insert(indoc);

            var result = inserts.FindOne(new Document().Add("x", 2));
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result["y"]);
        }

        [Test]
        public void TestSave()
        {
            var saves = DB["saves"];
            var count = 100;
            for(var i = 0; i < count; i++)
                saves.Save(new Document {{"x", i}, {"desc", "This document is number: " + i}, {"y", 1}});
            Assert.AreEqual(count, saves.Count(new Document {{"y", 1}}));

            using(var cur = saves.FindAll())
            {
                foreach(var d in cur.Documents)
                {
                    d["y"] = Convert.ToInt32(d["y"]) + 1;
                    saves.Save(d);
                }
            }
            Assert.AreEqual(count, saves.Count(new Document {{"y", 2}}));
        }

        [Test]
        public void TestSaveInsertDocumentIfExists()
        {
            var saves = DB["updates"];
            saves.Delete(new Document());

            var document1 = new Document("name", "Alien1");
            saves.Insert(document1);
            var document2 = new Document("name", "Alien2");
            saves.Insert(document2);

            document1["name"] = "Sam";
            saves.Save(document1);
            document2["name"] = "Steve";
            saves.Save(document2);

            var array = saves.FindAll().Documents.ToArray();
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual("Sam", array[0]["name"]);
            Assert.AreEqual("Steve", array[1]["name"]);
        }

        [Test]
        public void TestSaveInsertDocumentIfNotExists()
        {
            var saves = DB["updates"];
            saves.Delete(new Document());

            saves.Save(new Document("name", "Sam"));
            saves.Save(new Document("name", "Steve"));

            var array = saves.FindAll().Documents.ToArray();
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual("Sam", array[0]["name"]);
            Assert.AreEqual("Steve", array[1]["name"]);
        }

        [Test]
        public void TestSimpleInsert()
        {
            var inserts = DB["inserts"];
            var indoc = new Document();
            indoc["song"] = "Palmdale";
            indoc["artist"] = "Afroman";
            indoc["year"] = 1999;

            inserts.Insert(indoc);

            var result = inserts.FindOne(new Document().Add("song", "Palmdale"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1999, result["year"]);
        }

        [Test]
        public void TestUpdateMany()
        {
            var updates = DB["updates"];

            updates.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam"));
            updates.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam2"));
            updates.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam3"));

            var selector = new Document().Add("Last", "Cordr");
            var results = updates.Find(selector);
            var found = false;
            foreach(var doc in results.Documents)
            {
                Assert.AreEqual("Cordr", doc["Last"]);
                found = true;
            }
            Assert.IsTrue(found, "Should have found docs inserted for TestUpdateMany");
            Assert.AreEqual(3, updates.Count(selector), "Didn't find all Documents inserted for TestUpdateMany with Selector");

            //Document updateData = new Document().Append("$set", new Document().Append("Last", "Corder2"));
            var updateData = new Document().Add("Last", "Corder2");
            updates.UpdateAll(updateData, selector);

            selector["Last"] = "Corder2";
            Assert.AreEqual(3, updates.Count(selector), "Not all Cordr documents were updated");

            results = updates.Find(selector);
            found = false;
            foreach(var doc in results.Documents)
            {
                Assert.AreEqual("Corder2", doc["Last"]);
                Assert.IsNotNull(doc["First"], "First name should not disappear");
                found = true;
            }
            Assert.IsTrue(found, "Should have found docs updated for TestMany");
        }

        [Test]
        public void TestUpdatePartial()
        {
            var updates = DB["updates"];
            var coolness = 5;
            var einstein = new Document {{"Last", "Einstien"}, {"First", "Albert"}, {"Coolness", coolness++}};
            updates.Insert(einstein);
            var selector = new Document {{"_id", einstein["_id"]}};

            updates.Update(new Document {{"$inc", new Document {{"Coolness", 1}}}}, selector);
            Assert.AreEqual(coolness++, Convert.ToInt32(updates.FindOne(selector)["Coolness"]), "Coolness field not incremented", true);

            updates.Update(new Document
            {
                {"$set", new Document {{"Last", "Einstein"}}},
                {"$inc", new Document {{"Coolness", 1}}}
            },
                selector,
                true);
            Assert.AreEqual(coolness++, Convert.ToInt32(updates.FindOne(selector)["Coolness"]), "Coolness field not incremented");
        }

        [Test]
        public void TestUpdateUpsertExisting()
        {
            var updates = DB["updates"];
            var doc = new Document();
            doc["First"] = "Mtt";
            doc["Last"] = "Brewer";

            updates.Insert(doc);

            var selector = new Document().Add("Last", "Brewer");
            doc = updates.FindOne(selector);
            Assert.IsNotNull(doc);
            Assert.AreEqual("Mtt", doc["First"]);
            Assert.IsNotNull(doc["_id"]);

            doc["First"] = "Matt";
            updates.Update(doc);

            var result = updates.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual("Matt", result["First"]);
        }

        [Test]
        public void TestUpdateUpsertNotExisting()
        {
            var updates = DB["updates"];
            var doc = new Document();
            doc["First"] = "Sam";
            doc["Last"] = "CorderNE";

            updates.Update(doc);
            var selector = new Document().Add("Last", "CorderNE");
            var result = updates.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual("Sam", result["First"]);
        }

        [Test]
        public void TestWhere()
        {
            var c = DB["finds"].Find("this.j % 2 == 0");
            foreach(var result in c.Documents)
            {
                Assert.IsNotNull(result);
                var j = result["j"];
                Assert.IsTrue(Convert.ToInt32(j)%2 == 0);
            }
        }
    }
}