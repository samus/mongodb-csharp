using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Attributes;
using NUnit.Framework;

namespace MongoDB.IntegrationTests
{
    [TestFixture]
    public class TestCollection_1 : MongoTestBase
    {
        private const string POUND = "\u00a3";

        private class CountsEntity
        {
            public Oid Id { get; set; }

            public string Last { get; set; }

            public string First { get; set; }

            [MongoAlias("cnt")]
            public int Coolness { get; set; }
        }

        private class FindsEntity
        {
            public int x { get; set; }

            [MongoAlias("h")]
            public string Text { get; set; }

            [MongoAlias("j")]
            public int Index { get; set; }

            public int n { get; set; }
        }

        private class CharReadsEntity
        {
            public string test { get; set; }
        }

        private class InsertsEntity
        {
            [MongoAlias("song")]
            public string Song { get; set; }

            [MongoAlias("artist")]
            public string Artist { get; set; }

            [MongoAlias("year")]
            public int Year { get; set; }
        }

        private class Album
        {
            [MongoAlias("artist")]
            public string Artist { get; set; }

            [MongoAlias("title")]
            public string Title { get; set; }

            [MongoAlias("songs")]
            public List<Song> Songs { get; set; }
        }

        private class Song
        {
            [MongoAlias("title")]
            public string Title { get; set; }

            [MongoAlias("length")]
            public string Length { get; set; }
        }

        private class AlbumCase
        {
            public AlbumCase(){
                Album = new Album();
            }

            public Album Album { get; set; }
        }

        private class DeletesEntity
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public override string TestCollections{
            get { return "inserts,updates,counts,counts_spec,finds,charreads,saves"; }
        }

        public override void OnInit(){
            var finds = DB["finds"];
            for(var j = 1; j < 100; j++)
                finds.Insert(new Document {{"x", 4}, {"h", "hi"}, {"j", j}});
            for(var j = 100; j < 105; j++)
                finds.Insert(new Document {{"x", 4}, {"n", 1}, {"j", j}});
            var charreads = DB["charreads"];
            charreads.Insert(new Document {{"test", "1234" + POUND + "56"}});
        }

        [Test]
        public void TestArrayInsert(){
            var inserts = DB.GetCollection<InsertsEntity>("inserts");
            var indoc1 = new {Song = "The Axe", Artist = "Tinsley Ellis", Year = 2006};
            var indoc2 = new {Song = "The Axe2", Artist = "Tinsley Ellis2", Year = 2008};

            inserts.Insert(new[] {indoc1, indoc2});

            var result = inserts.FindOne(new Document().Add("Song", "The Axe"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2006, result.Year);

            result = inserts.FindOne(new Document().Add("Song", "The Axe2"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2008, result.Year);
        }

        [Test]
        public void TestCanInsertNullPropertys(){
            var inserts = DB.GetCollection<CharReadsEntity>("inserts");

            inserts.Insert(new CharReadsEntity());
        }

        [Test]
        public void TestCount(){
            var counts = DB.GetCollection<CountsEntity>("counts");
            var top = 100;
            for(var i = 0; i < top; i++)
                counts.Insert(new CountsEntity {Last = "Cordr", First = "Sam", Coolness = i});
            var cnt = counts.Count();
            Assert.AreEqual(top, cnt, "Count not the same as number of inserted records");
        }

        [Test]
        public void TestCountInvalidCollection(){
            var counts = DB.GetCollection<CountsEntity>("counts_wtf");
            Assert.AreEqual(0, counts.Count());
        }

        [Test]
        public void TestCountWithSpec(){
            var counts = DB.GetCollection<CountsEntity>("counts_spec");
            counts.Insert(new CountsEntity {Last = "Cordr", First = "Sam", Coolness = 1});
            counts.Insert(new CountsEntity {Last = "Cordr", First = "Sam", Coolness = 2});
            counts.Insert(new CountsEntity {Last = "Corder", First = "Sam", Coolness = 3});

            Assert.AreEqual(2, counts.Count(new {Last = "Cordr"}));
            Assert.AreEqual(1, counts.Count(new {Last = "Corder"}));
            Assert.AreEqual(0, counts.Count(new {Last = "Brown"}));
        }

        [Test]
        public void TestDelete(){
            var deletes = DB.GetCollection<DeletesEntity>("deletes");
            deletes.Insert(new {x = 2, y = 1});

            var selector = new {x = 2};

            var result = deletes.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.y);

            deletes.Delete(selector);
            result = deletes.FindOne(selector);
            Assert.IsNull(result, "Shouldn't have been able to find a document that was deleted");
        }

        [Test]
        public void TestFindAttributeLimit(){
            var query = new {Index = 10};
            var fields = new {x = 1};
            var c = DB.GetCollection<FindsEntity>("finds").Find(query, -1, 0, fields);
            foreach(var result in c.Documents)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(4, result.x);
                Assert.AreEqual(0, result.Index);
            }
        }

        [Test]
        public void TestFindGTRange(){
            var query = new {Index = Op.GreaterThan(20)};
            var c = DB.GetCollection<FindsEntity>("finds").Find(query);
            foreach(var result in c.Documents)
            {
                Assert.IsNotNull(result);
                Assert.Greater(result.Index, 20);
            }
        }

        [Test]
        public void TestFindNulls(){
            var query = new {Text = (string)null};
            var numnulls = DB.GetCollection<FindsEntity>("finds").Count(query);
            Assert.AreEqual(5, numnulls);
        }

        [Test]
        public void TestFindOne(){
            var query = new {Index = 10};
            var result = DB.GetCollection<FindsEntity>("finds").FindOne(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(10, result.Index);
        }

        [Test]
        public void TestFindOneNotThere(){
            var query = new {not_there = 10};
            var result = DB.GetCollection<FindsEntity>("finds").FindOne(query);
            Assert.IsNull(result);
        }

        [Test]
        public void TestFindOneObjectContainingUKPound(){
            var query = new Document();
            var result = DB.GetCollection<CharReadsEntity>("charreads").FindOne(query);
            Assert.IsNotNull(result);
            Assert.AreEqual("1234£56", result.test);
        }

        [Test]
        public void TestFindWhereEquivalency(){
            var col = DB.GetCollection<FindsEntity>("finds");
            var lt = new {Index = Op.LessThan(5)};
            var where = "this.j < 5";
            var explicitWhere = new Document().Add("$where", new Code(where));
            var func = new CodeWScope("function() { return this.j < 5; }", new Document());
            var funcDoc = new Document().Add("$where", func);

            Assert.AreEqual(4, col.Find(lt).Documents.Count(), "Basic find didn't return 4 docs");
            Assert.AreEqual(4, col.Find(where).Documents.Count(), "String where didn't return 4 docs");
            Assert.AreEqual(4, col.Find(explicitWhere).Documents.Count(), "Explicit where didn't return 4 docs");
            Assert.AreEqual(4, col.Find(funcDoc).Documents.Count(), "Function where didn't return 4 docs");
        }

        [Test]
        public void TestInsertBulkLargerThan4MBOfDocuments(){
            var b = new Binary(new byte[1024*1024*2]);
            var inserts = DB.GetCollection<InsertsEntity>("inserts");
            try
            {
                //6MB+ of documents
                var docs = from i in Enumerable.Range(1, 10)
                           select new {Song = "Bulk", bin = b, Year = i};

                inserts.Insert(docs, true);
                var count = inserts.Count(new Document("Song", "Bulk"));
                Assert.AreEqual(docs.Count(), count, "Wrong number of documents inserted");
            }
            catch(MongoException)
            {
                Assert.Fail("MongoException should not have been thrown.");
            }
        }

        [Test]
        public void TestInsertOfArray(){
            var ogen = new OidGenerator();
            var inserts = DB.GetCollection<Album>("inserts");
            var album = new Album {Title = "Deliveries After Dark", Artist = "Popa Chubby"};
            album.Songs = new List<Song>
            {
                new Song {Title = "Let The Music Set You Free", Length = "5:15"},
                new Song {Title = "Sally Likes to Run", Length = "4:06"},
                new Song {Title = "Deliveries After Dark", Length = "4:17"},
                new Song {Title = "Theme From The Godfather", Length = "3:06"},
                new Song {Title = "Grown Man Crying Blues", Length = "8:09"}
            };
            inserts.Insert(album);

            var result = inserts.FindOne(new Document().Add("songs.title", "Deliveries After Dark"));
            Assert.IsNotNull(result);

            Assert.AreEqual(album.Songs.Count, result.Songs.Count);
        }

        [Test]
        public void TestSimpleInsert(){
            var inserts = DB.GetCollection<InsertsEntity>("inserts");
            var indoc = new InsertsEntity {Artist = "Afroman", Song = "Palmdale", Year = 1999};
            inserts.Insert(indoc);

            var result = inserts.FindOne(new {Song = "Palmdale"});
            Assert.IsNotNull(result);
            Assert.AreEqual(indoc.Year, result.Year);
        }

        [Test]
        public void TestUpdateMany(){
            var updates = DB.GetCollection<CountsEntity>("updates");

            updates.Insert(new CountsEntity {Last = "Cordr", First = "Sam"});
            updates.Insert(new CountsEntity {Last = "Cordr", First = "Sam2"});
            updates.Insert(new CountsEntity {Last = "Cordr", First = "Sam3"});

            var selector = new {Last = "Cordr"};
            var results = updates.Find(selector);
            Assert.AreEqual(3, results.Documents.Count(), "Didn't find all Documents inserted for TestUpdateMany with Selector");

            var updateData = new {Last = "Cordr2"};
            updates.UpdateAll(updateData, selector);

            selector = new {Last = "Cordr2"};
            results = updates.Find(selector);
            var count = 0;
            foreach(var doc in results.Documents)
            {
                count++;
                Assert.AreEqual("Cordr2", doc.Last);
                Assert.IsNotNull(doc.First, "First name should not disappear");
            }

            Assert.AreEqual(3, count, "Didn't find all documents for updated.");
        }

        [Test]
        public void TestUpdatePartial(){
            var updates = DB.GetCollection<CountsEntity>("updates");
            var coolness = 5;
            var einstein = new CountsEntity {Last = "Einstein", First = "Albret", Coolness = coolness++};
            updates.Insert(einstein);
            var selector = new {Last = "Einstein"};

            updates.Update(new Document {{"$inc", new Document("cnt", 1)}}, selector);
            Assert.AreEqual(coolness++, Convert.ToInt32(updates.FindOne(selector).Coolness), "Coolness field not incremented", true);

            updates.Update(new Document
            {
                {"$set", new {First = "Albert"}},
                {"$inc", new Document {{"cnt", 1}}}
            },
                selector,
                true);
            Assert.AreEqual(coolness++, Convert.ToInt32(updates.FindOne(selector).Coolness), "Coolness field not incremented");
        }

        [Test]
        public void TestUpdateUpsertExisting(){
            var updates = DB.GetCollection<CountsEntity>("updates");
            var doc = new CountsEntity {First = "Mtt", Last = "Brewer"};

            updates.Insert(doc);

            var selector = new {Last = "Brewer"};
            doc = updates.FindOne(selector);
            Assert.IsNotNull(doc);
            Assert.AreEqual("Mtt", doc.First);
            Assert.IsNotNull(doc.Id);

            doc.First = "Matt";
            updates.Update(doc);

            var result = updates.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual("Matt", result.First);
        }

        [Test]
        public void TestUpdateUpsertNotExisting(){
            var updates = DB.GetCollection<CountsEntity>("updates");
            var doc = new CountsEntity {First = "Sam", Last = "CorderNE"};

            updates.Update(doc);
            var result = updates.FindOne(new {Last = "CorderNE"});
            Assert.IsNotNull(result);
            Assert.AreEqual("Sam", result.First);
        }

        [Test]
        public void CanSaveNewDocumentsWithoutId(){
            var saves = DB.GetCollection<Document>("saves");
            saves.Save(new Document("WithoutId", 1.0));

            var result = saves.FindOne(new Document("WithoutId",1.0));
            Assert.IsNotNull(result);
        }

        [Test]
        public void CanSaveNewDocumentWithId(){
            var saves = DB.GetCollection<Document>("saves");
            saves.Save(new Document("WithId", 1.0).Add("_id", 5));

            var result = saves.FindOne(new Document("_id", 5));
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id,5);
        }

        [Test]
        public void SaveUpdatesExistsingDocument(){
            var saves = DB.GetCollection<Document>("saves");
            var updated = new Document("Existing", 1.0);
            saves.Insert(updated);

            updated["Existing"] = 2.0;

            saves.Save(updated);

            var result = saves.FindOne(new Document("_id", updated.Id));
            Assert.IsNotNull(result);
            Assert.AreEqual(result["Existing"], 2.0);
        }
    }
}