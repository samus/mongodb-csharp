using System;
using System.Collections.Generic;

using NUnit.Framework;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollection : MongoTestBase
    {
        private string pound = "\u00a3";

        public override string TestCollections {
            get {
                return "inserts,updates,counts,counts_spec,finds,charreads";
            }
        }

        public override void OnInit (){
            IMongoCollection<Document> finds = DB["finds"];
            for(int j = 1; j < 100; j++){
                finds.Insert(new Document(){{"x", 4},{"h", "hi"},{"j", j}});
            }
            for(int j = 100; j < 105; j++){
                finds.Insert(new Document(){{"x", 4},{"n", 1},{"j", j}});
            }
            IMongoCollection<Document> charreads = DB["charreads"];
            charreads.Insert(new Document(){{"test", "1234" + pound + "56"}});

        }


        [Test]
        public void TestFindOne(){
            Document query = new Document();
            query["j"] = 10;
            Document result = DB["finds"].FindOne(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result["x"]);
            Assert.AreEqual(10, result["j"]);

        }

        [Test]
        public void TestFindOneNotThere(){
            Document query = new Document();
            query["not_there"] = 10;
            Document result = DB["finds"].FindOne(query);
            Assert.IsNull(result);
        }

        [Test]
        public void TestFindNulls(){
            Document query = new Document().Add("n", null);
            long numnulls = DB["finds"].Count(query);
            Assert.AreEqual(99,numnulls);
        }

        [Test]
        public void TestFindAttributeLimit(){
            Document query = new Document();
            query["j"] = 10;
            Document fields = new Document();
            fields["x"] = 1;

            ICursor<Document> c = DB["finds"].Find(query, -1, 0, fields);
            foreach(Document result in c.Documents){
                Assert.IsNotNull(result);
                Assert.AreEqual(4, result["x"]);
                Assert.IsNull(result["j"]);
            }
        }

        [Test]
        public void TestFindGTRange(){
            Document query = new Document();
            query["j"] = new Document().Add("$gt", 20);

            ICursor<Document> c = DB["finds"].Find(query);
            foreach(Document result in c.Documents){
                Assert.IsNotNull(result);
                Object j = result["j"];
                Assert.IsTrue(Convert.ToDouble(j) > 20);
            }
        }

        [Test]
        public void TestManualWhere(){
            Document query = new Document().Add("$where", new Code("this.j % 2 == 0"));
            ICursor<Document> c = DB["finds"].Find(query);
            foreach(Document result in c.Documents){
                Assert.IsNotNull(result);
                Object j = result["j"];
                Assert.IsTrue(Convert.ToInt32(j) % 2 == 0);
            }
        }
        [Test]
        public void TestFindWhereEquivalency(){
            IMongoCollection<Document> col = DB["finds"];
            Document lt = new Document().Add("j", new Document().Add("$lt", 5));
            string where = "this.j < 5";
            Document explicitWhere = new Document().Add("$where", new Code(where));
            CodeWScope func = new CodeWScope("function() { return this.j < 5; }", new Document());
            Document funcDoc = new Document().Add("$where", func);

            Assert.AreEqual(4, CountDocs(col.Find(lt)), "Basic find didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(where)), "String where didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(explicitWhere)), "Explicit where didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(funcDoc)), "Function where didn't return 4 docs");
        }

        private int CountDocs(ICursor<Document> cur)
        {
            int cnt = 0;
            foreach(Document doc in cur.Documents){
                cnt++;
            }
            return cnt;
        }
        [Test]
        public void TestWhere(){
            ICursor<Document> c = DB["finds"].Find("this.j % 2 == 0");
            foreach(Document result in c.Documents){
                Assert.IsNotNull(result);
                Object j = result["j"];
                Assert.IsTrue(Convert.ToInt32(j) % 2 == 0);
            }
        }

        [Test]
        public void TestFindOneObjectContainingUKPound(){
            Document query = new Document();
            Document result = DB["charreads"].FindOne(query);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("test"));
            Assert.AreEqual("1234£56",result["test"]);
        }

        [Test]
        public void TestSimpleInsert(){
            IMongoCollection<Document> inserts = DB["inserts"];
            Document indoc = new Document();
            indoc["song"] = "Palmdale";
            indoc["artist"] = "Afroman";
            indoc["year"] = 1999;

            inserts.Insert(indoc);

            Document result = inserts.FindOne(new Document().Add("song", "Palmdale"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1999,result["year"]);
        }

        [Test]
        public void TestReallySimpleInsert(){
            IMongoCollection<Document> inserts = DB["inserts"];
            Document indoc = new Document();
            indoc["y"] = 1;
            indoc["x"] = 2;
            inserts.Insert(indoc);

            Document result = inserts.FindOne(new Document().Add("x", 2));
            Assert.IsNotNull(result);
            Assert.AreEqual(1,result["y"]);
        }

        [Test]
        public void TestPoundSymbolInsert(){
            IMongoCollection<Document> inserts = DB["inserts"];
            Document indoc = new Document().Add("x", "1234" + pound + "56").Add("y", 1);
            inserts.Insert(indoc);

            Document result = inserts.FindOne(new Document().Add("x", "1234" + pound + "56"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1,result["y"]);
        }

        [Test]
        public void TestArrayInsert(){
            IMongoCollection<Document> inserts = DB["inserts"];
            Document indoc1 = new Document();
            indoc1["song"] = "The Axe";
            indoc1["artist"] = "Tinsley Ellis";
            indoc1["year"] = 2006;

            Document indoc2 = new Document();
            indoc2["song"] = "The Axe2";
            indoc2["artist"] = "Tinsley Ellis2";
            indoc2["year"] = 2008;

            inserts.Insert(new Document[]{indoc1,indoc2});

            Document result = inserts.FindOne(new Document().Add("song", "The Axe"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2006,result["year"]);

            result = inserts.FindOne(new Document().Add("song", "The Axe2"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2008,result["year"]);
        }

        [Test]
        public void TestInsertOfArray(){
            OidGenerator ogen = new OidGenerator();
            IMongoCollection<Document> inserts = DB["inserts"];
            Document album = new Document();
            album["_id"] = ogen.Generate();
            album["artist"] = "Popa Chubby";
            album["title"] = "Deliveries After Dark";
            album["songs"] = new[] {
                new Document().Add("title", "Let The Music Set You Free").Add("length", "5:15").Add("_id", ogen.Generate()),
                new Document().Add("title", "Sally Likes to Run").Add("length", "4:06").Add("_id", ogen.Generate()),
                new Document().Add("title", "Deliveries After Dark").Add("length", "4:17").Add("_id", ogen.Generate()),
                new Document().Add("title", "Theme From The Godfather").Add("length", "3:06").Add("_id", ogen.Generate()),
                new Document().Add("title", "Grown Man Crying Blues").Add("length", "8:09").Add("_id", ogen.Generate()),
            };
            inserts.Insert(album);

            Document result = inserts.FindOne(new Document().Add("songs.title", "Deliveries After Dark"));
            Assert.IsNotNull(result);

            Assert.AreEqual(album.ToString(), result.ToString());
        }

        [Test]
        public void TestInsertLargerThan4MBDocument(){
            Binary b = new Binary(new byte[1024 * 1024]);
            Document big = new Document(){{"name", "Big Document"}, {"b1", b}, {"b2", b}, {"b3", b}, {"b4", b}};
            IMongoCollection<Document> inserts = DB["inserts"];
            bool thrown = false;
            try{
                inserts.Insert(big);               
            }catch(MongoException){
                thrown = true;
            }catch(Exception e){
                Assert.Fail("Wrong Exception thrown " + e.GetType().Name);
            }
            Assert.IsTrue(thrown, "Shouldn't be able to insert large document");
        }
        
        [Test]
        public void TestInsertBulkLargerThan4MBOfDocuments(){
            Binary b = new Binary(new byte[1024 * 1024 * 2]);
            IMongoCollection<Document> inserts = DB["inserts"];
            try{
                Document[] docs = new Document[10];
                    //6MB+ of documents
                for(int x = 0; x < docs.Length; x++){
                    docs[x] = new Document(){{"name", "bulk"}, {"b", b}, {"x", x}};
                }
                inserts.Insert(docs,true);
                long count = inserts.Count(new Document(){{"name", "bulk"}});
                Assert.AreEqual(docs.Length, count, "Wrong number of documents inserted");
            }catch(MongoException){
                Assert.Fail("MongoException should not have been thrown.");
            }
        }
        
        [Test]
        public void TestDelete(){
            IMongoCollection<Document> deletes = DB["deletes"];
            Document doc = new Document();
            doc["y"] = 1;
            doc["x"] = 2;
            deletes.Insert(doc);

            Document selector = new Document().Add("x", 2);

            Document result = deletes.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual(1,result["y"]);

            deletes.Delete(selector);
            result = deletes.FindOne(selector);
            Assert.IsNull(result,"Shouldn't have been able to find a document that was deleted");

        }

        [Test]
        public void TestUpdateUpsertNotExisting(){
            IMongoCollection<Document> updates = DB["updates"];
            Document doc = new Document();
            doc["First"] = "Sam";
            doc["Last"] = "CorderNE";

            updates.Update(doc);
            Document selector = new Document().Add("Last", "CorderNE");
            Document result = updates.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual("Sam", result["First"]);
        }

        [Test]
        public void TestUpdateUpsertExisting(){
            IMongoCollection<Document> updates = DB["updates"];
            Document doc = new Document();
            doc["First"] = "Mtt";
            doc["Last"] = "Brewer";

            updates.Insert(doc);

            Document selector = new Document().Add("Last", "Brewer");
            doc = updates.FindOne(selector);
            Assert.IsNotNull(doc);
            Assert.AreEqual("Mtt", doc["First"]);
            Assert.IsNotNull(doc["_id"]);

            doc["First"] = "Matt";
            updates.Update(doc);

            Document result = updates.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual("Matt", result["First"]);

        }

        [Test]
        public void TestUpdateMany(){
            IMongoCollection<Document> updates = DB["updates"];

            updates.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam"));
            updates.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam2"));
            updates.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam3"));

            Document selector = new Document().Add("Last", "Cordr");
            ICursor<Document> results = updates.Find(selector);
            bool found = false;
            foreach(Document doc in results.Documents){
                Assert.AreEqual("Cordr", doc["Last"]);
                found = true;
            }
            Assert.IsTrue(found,"Should have found docs inserted for TestUpdateMany");
            Assert.AreEqual(3, updates.Count(selector), "Didn't find all Documents inserted for TestUpdateMany with Selector");

            //Document updateData = new Document().Append("$set", new Document().Append("Last", "Corder2"));
            Document updateData = new Document().Add("Last", "Corder2");
            updates.UpdateAll(updateData, selector);

            selector["Last"] = "Corder2";
            Assert.AreEqual(3, updates.Count(selector), "Not all Cordr documents were updated");

            results = updates.Find(selector);
            found = false;
            foreach(Document doc in results.Documents){
                Assert.AreEqual("Corder2", doc["Last"]);
                Assert.IsNotNull(doc["First"],"First name should not disappear");
                found = true;
            }
            Assert.IsTrue(found,"Should have found docs updated for TestMany");
        }

        [Test]
        public void TestUpdatePartial(){
            IMongoCollection<Document> updates = DB["updates"];
            int coolness = 5;
            Document einstein = new Document(){{"Last", "Einstien"},{"First", "Albert"},{"Coolness",coolness++}};
            updates.Insert(einstein);
            Document selector = new Document(){{"_id", einstein["_id"]}};
            
            updates.Update(new Document(){{"$inc", new Document(){{"Coolness", 1}}}}, selector);
            Assert.AreEqual(coolness++, Convert.ToInt32(updates.FindOne(selector)["Coolness"]), "Coolness field not incremented", true);
            
            updates.Update(new Document(){{"$set",new Document(){{"Last", "Einstein"}}},
                                          {"$inc",new Document(){{"Coolness",1}}}},selector,true);
            Assert.AreEqual(coolness++, Convert.ToInt32(updates.FindOne(selector)["Coolness"]), "Coolness field not incremented");
        }
        
        [Test]
        public void TestCount(){
            IMongoCollection<Document> counts = DB["counts"];
            int top = 100;
            for(int i = 0; i < top; i++){
                counts.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam").Add("cnt", i));
            }
            long cnt = counts.Count();
            Assert.AreEqual(top,cnt, "Count not the same as number of inserted records");
        }

        [Test]
        public void TestCountWithSpec(){
            IMongoCollection<Document> counts = DB["counts_spec"];
            counts.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam").Add("cnt", 1));
            counts.Insert(new Document().Add("Last", "Cordr").Add("First", "Sam").Add("cnt", 2));
            counts.Insert(new Document().Add("Last", "Corder").Add("First", "Sam").Add("cnt", 3));

            Assert.AreEqual(2, counts.Count(new Document().Add("Last", "Cordr")));
            Assert.AreEqual(1, counts.Count(new Document().Add("Last", "Corder")));
            Assert.AreEqual(0, counts.Count(new Document().Add("Last", "Brown")));

        }

        [Test]
        public void TestCountInvalidCollection(){
            IMongoCollection<Document> counts = DB["counts_wtf"];
            Assert.AreEqual(0, counts.Count());
        }
    }
}