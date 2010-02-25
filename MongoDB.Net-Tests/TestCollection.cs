using System;
using NUnit.Framework;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollection
    {
        Mongo db = new Mongo();

        [Test]
        public void TestFindOne(){
            Document query = new Document();
            query["j"] = 10;
            Document result = db["tests"]["reads"].FindOne(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result["x"]);
            Assert.AreEqual(10, result["j"]);
            
        }
        
        [Test]
        public void TestFindOneNotThere(){
            Document query = new Document();
            query["not_there"] = 10;
            Document result = db["tests"]["reads"].FindOne(query);
            Assert.IsNull(result);
        }
        
        [Test]
        public void TestFindNulls(){
            Document query = new Document().Append("n",MongoDBNull.Value);
            long numnulls = db["tests"]["smallreads"].Count(query);
            Assert.AreEqual(4,numnulls);
        }
        
        [Test]
        public void TestFindAttributeLimit(){
            Document query = new Document();
            query["j"] = 10;
            Document fields = new Document();
            fields["x"] = 1;
                        
            ICursor c = db["tests"]["reads"].Find(query,-1,0,fields);
            foreach(Document result in c.Documents){            
                Assert.IsNotNull(result);
                Assert.AreEqual(4, result["x"]);
                Assert.IsNull(result["j"]);                         
            }
        }
        
        [Test]
        public void TestFindGTRange(){
            Document query = new Document();
            query["j"] = new Document().Append("$gt",20);
            
            ICursor c = db["tests"]["reads"].Find(query);
            foreach(Document result in c.Documents){            
                Assert.IsNotNull(result);
                Object j = result["j"];
                Assert.IsTrue(Convert.ToDouble(j) > 20);
            }           
        }
        
        [Test]
        public void TestManualWhere(){
            Document query = new Document().Append("$where", new Code("this.j % 2 == 0"));
            ICursor c = db["tests"]["reads"].Find(query);
            foreach(Document result in c.Documents){            
                Assert.IsNotNull(result);
                Object j = result["j"];
                Assert.IsTrue((double)j % 2 == 0);
            }                       
        }
        [Test]
        public void TestFindWhereEquivalency(){
            IMongoCollection col = db["tests"]["reads"];
            Document lt = new Document().Append("j", new Document().Append("$lt", 5));
            string where = "this.j < 5";
            Document explicitWhere = new Document().Append("$where", new Code(where));
            CodeWScope func = new CodeWScope("function() { return this.j < 5; }", new Document());
            Document funcDoc = new Document().Append("$where", func);
            
            Assert.AreEqual(4, CountDocs(col.Find(lt)), "Basic find didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(where)), "String where didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(explicitWhere)), "Explicit where didn't return 4 docs");
            Assert.AreEqual(4, CountDocs(col.Find(funcDoc)), "Function where didn't return 4 docs");
        }
        
        private int CountDocs(ICursor cur){
            int cnt = 0;
            foreach(Document doc in cur.Documents){
                cnt++;
            }
            return cnt;
        }
        [Test]
        public void TestWhere(){
            ICursor c = db["tests"]["reads"].Find("this.j % 2 == 0");
            foreach(Document result in c.Documents){            
                Assert.IsNotNull(result);
                Object j = result["j"];
                Assert.IsTrue((double)j % 2 == 0);
            }                       
        }        

        [Test]
        public void TestFindOneObjectContainingUKPound(){
            Document query = new Document();
            Document result = db["tests"]["charreads"].FindOne(query);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("test"));
            Assert.AreEqual("1234£56",result["test"]);
        }
        
        [Test]
        public void TestSimpleInsert(){
            IMongoCollection inserts = db["tests"]["inserts"];
            Document indoc = new Document();
            indoc["song"] = "Palmdale";
            indoc["artist"] = "Afroman";
            indoc["year"] = 1999;

            inserts.Insert(indoc);
            
            Document result = inserts.FindOne(new Document().Append("song","Palmdale"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1999,result["year"]);
        }
        
        [Test]
        public void TestReallySimpleInsert(){
            IMongoCollection inserts = db["tests"]["inserts"];
            Document indoc = new Document();
            indoc["y"] = 1;
            indoc["x"] = 2;
            inserts.Insert(indoc);
            
            Document result = inserts.FindOne(new Document().Append("x",2));
            Assert.IsNotNull(result);
            Assert.AreEqual(1,result["y"]);
        }
        
        [Test]
        public void TestPoundSymbolInsert(){
            IMongoCollection inserts = db["tests"]["inserts"];
            Document indoc = new Document().Append("x","1234£56").Append("y",1);;
            inserts.Insert(indoc);

            Document result = inserts.FindOne(new Document().Append("x","1234£56"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1,result["y"]);            
        }
        
        [Test]
        public void TestArrayInsert(){
            IMongoCollection inserts = db["tests"]["inserts"];
            Document indoc1 = new Document();
            indoc1["song"] = "The Axe";
            indoc1["artist"] = "Tinsley Ellis";
            indoc1["year"] = 2006;

            Document indoc2 = new Document();
            indoc2["song"] = "The Axe2";
            indoc2["artist"] = "Tinsley Ellis2";
            indoc2["year"] = 2008;
            
            inserts.Insert(new Document[]{indoc1,indoc2});
            
            Document result = inserts.FindOne(new Document().Append("song","The Axe"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2006,result["year"]);
            
            result = inserts.FindOne(new Document().Append("song","The Axe2"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2008,result["year"]);           
        }       
        
        [Test]
        public void TestInsertOfArray(){
            OidGenerator ogen = new OidGenerator();
            IMongoCollection inserts = db["tests"]["inserts"];
            Document album = new Document();
            album["_id"] = ogen.Generate();
            album["artist"] = "Popa Chubby";
            album["title"] = "Deliveries After Dark";
            album["songs"] = new[] {
                new Document().Append("title", "Let The Music Set You Free").Append("length", "5:15").Append("_id", ogen.Generate()),
                new Document().Append("title", "Sally Likes to Run").Append("length", "4:06").Append("_id", ogen.Generate()),
                new Document().Append("title", "Deliveries After Dark").Append("length", "4:17").Append("_id", ogen.Generate()),
                new Document().Append("title", "Theme From The Godfather").Append("length", "3:06").Append("_id", ogen.Generate()),
                new Document().Append("title", "Grown Man Crying Blues").Append("length", "8:09").Append("_id", ogen.Generate()),
            };
            inserts.Insert(album);
			
            Document result = inserts.FindOne(new Document().Append("songs.title","Deliveries After Dark"));
            Assert.IsNotNull(result);
            
            Assert.AreEqual(album.ToString(), result.ToString());
        }
				
        [Test]
        public void TestDelete(){
            IMongoCollection deletes = db["tests"]["deletes"];
            Document doc = new Document();
            doc["y"] = 1;
            doc["x"] = 2;
            deletes.Insert(doc);
            
            Document selector = new Document().Append("x",2);
            
            Document result = deletes.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual(1,result["y"]);
            
            deletes.Delete(selector);
            result = deletes.FindOne(selector);
            Assert.IsNull(result,"Shouldn't have been able to find a document that was deleted");
            
        }       
        
        [Test]
        public void TestUpdateUpsertNotExisting(){
            IMongoCollection updates = db["tests"]["updates"];
            Document doc = new Document();
            doc["First"] = "Sam";
            doc["Last"] = "CorderNE";
            
            updates.Update(doc);
            Document selector = new Document().Append("Last", "CorderNE");
            Document result = updates.FindOne(selector);
            Assert.IsNotNull(result);
            Assert.AreEqual("Sam", result["First"]);
        }
        
        [Test]
        public void TestUpdateUpsertExisting(){
            IMongoCollection updates = db["tests"]["updates"];
            Document doc = new Document();
            doc["First"] = "Mtt";
            doc["Last"] = "Brewer";
            
            updates.Insert(doc);
            
            Document selector = new Document().Append("Last", "Brewer");
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
            IMongoCollection updates = db["tests"]["updates"];
            
            updates.Insert(new Document().Append("Last", "Cordr").Append("First","Sam"));
            updates.Insert(new Document().Append("Last", "Cordr").Append("First","Sam2"));
            updates.Insert(new Document().Append("Last", "Cordr").Append("First","Sam3"));
            
            Document selector = new Document().Append("Last", "Cordr");
            ICursor results = updates.Find(selector);
            bool found = false;
            foreach(Document doc in results.Documents){
                Assert.AreEqual("Cordr", doc["Last"]);
                found = true;
            }
            Assert.IsTrue(found,"Should have found docs inserted for TestUpdateMany");
            Assert.AreEqual(3, updates.Count(selector), "Didn't find all Documents inserted for TestUpdateMany with Selector");
            
            //Document updateData = new Document().Append("$set", new Document().Append("Last", "Corder2"));
            Document updateData = new Document().Append("Last", "Corder2");
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
        public void TestCount(){
            IMongoCollection counts = db["tests"]["counts"];
            int top = 100;
            for(int i = 0; i < top; i++){
                counts.Insert(new Document().Append("Last", "Cordr").Append("First","Sam").Append("cnt", i));
            }
            long cnt = counts.Count();
            Assert.AreEqual(top,cnt, "Count not the same as number of inserted records");
        }
        
        [Test]
        public void TestCountWithSpec(){
            IMongoCollection counts = db["tests"]["counts_spec"];
            counts.Insert(new Document().Append("Last", "Cordr").Append("First","Sam").Append("cnt", 1));
            counts.Insert(new Document().Append("Last", "Cordr").Append("First","Sam").Append("cnt", 2));
            counts.Insert(new Document().Append("Last", "Corder").Append("First","Sam").Append("cnt", 3));
            
            Assert.AreEqual(2, counts.Count(new Document().Append("Last", "Cordr")));
            Assert.AreEqual(1, counts.Count(new Document().Append("Last", "Corder")));
            Assert.AreEqual(0, counts.Count(new Document().Append("Last", "Brown")));
                            
        }
        
        [Test]
        public void TestCountInvalidCollection(){
            IMongoCollection counts = db["tests"]["counts_wtf"];
            Assert.AreEqual(0, counts.Count());
        }
        
        [TestFixtureSetUp]
        public void Init(){
            db.Connect();
            cleanDB();
        }
        
        [TestFixtureTearDown]
        public void Dispose(){
            //cleanDB();
            db.Disconnect();
        }
        
        protected void cleanDB(){
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","inserts"));
            
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","updates"));
            
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","counts"));
            
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","counts_spec"));
        }
    }
}