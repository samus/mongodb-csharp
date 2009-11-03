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
                Assert.IsTrue((double)j > 20);
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
        public void TestWhere(){
            ICursor c = db["tests"]["reads"].Find("this.j % 2 == 0");
            foreach(Document result in c.Documents){            
                Assert.IsNotNull(result);
                Object j = result["j"];
                Assert.IsTrue((double)j % 2 == 0);
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
            doc["Last"] = "Corder";
            
            updates.Update(doc);
            Document selector = new Document().Append("Last", "Corder");
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
            
            Document selector = new Document().Append("Last", "Cordr");
            ICursor results = updates.Find(selector);
            bool found = false;
            foreach(Document doc in results.Documents){
                Assert.AreEqual("Cordr", doc["Last"]);
                found = true;
            }
            Assert.IsTrue(found,"Should have found docs inserted for TestMany");
            
            Document updateData = new Document().Append("Last", "Corder2");
            updates.UpdateAll(updateData, selector);
            
            selector["Last"] = "Corder2";
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
