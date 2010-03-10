using System;
using NUnit.Framework;

using MongoDB.Driver;
using MongoDB.Driver.IO;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCursor
    {
        Mongo db = new Mongo();
        
        [Test]
        public void TestCanReadSmall()
        {
            ICursor c = db["tests"]["smallreads"].FindAll();
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            int reads = 0;
            foreach(Document doc in c.Documents){
                reads++;
            }
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.AreEqual(5, reads, "More than 5 documents in the small reads dataset");
        }
        
        [Test]
        public void TestCanReadMore(){
            ICursor c = db["tests"]["reads"].FindAll();
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            int reads = 0;
            int idchanges = 0;
            long id = 0;
            foreach(Document doc in c.Documents){
                reads++;
                if(c.Id != id){
                    idchanges++;
                    id = c.Id;
                }
            }
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.IsTrue(idchanges > 0,String.Format("ReadMore message never sent. {0} changes seen", idchanges));
            Assert.AreEqual(9999,reads, "Not all documents returned.");
            System.Console.Out.Write(String.Format("{0} records read", reads));
            

        }
        [Test]
        public void TestCanReadAndKillCursor()
        {
            ICursor c = db["tests"]["reads"].FindAll();
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            foreach(Document doc in c.Documents){
                break;
            }
            c.Dispose();
            Assert.AreEqual(0,c.Id);
        }
        
        [Test]
        public void TestCanLimit(){
            ICursor c = db["tests"]["reads"].FindAll().Limit(5);
            
            Assert.IsNotNull(c,"Cursor shouldn't be null");
            int reads = 0;
            foreach(Document doc in c.Documents){
                reads++;
            }
            Assert.IsTrue(reads > 0, "No documents were returned.");
            Assert.AreEqual(5, reads);
        }
        
        [Test]
        public void TestSort(){
            IMongoCollection sorts = db["tests"]["sorts"];
            int[] randoms = new int[]{4,6,8,9,1,3,2,5,7,0};
            foreach(int x in randoms){
                sorts.Insert(new Document().Append("x", randoms[x]));
            }
            Assert.AreEqual(randoms.Length, sorts.Count());
            
            int exp = 0;
            foreach(Document doc in sorts.FindAll().Sort("x", IndexOrder.Ascending).Documents){
                Assert.AreEqual(exp, Convert.ToInt32(doc["x"]));
                exp++;
            }
            
            exp = 9;
            foreach(Document doc in sorts.FindAll().Sort("x", IndexOrder.Descending).Documents){
                Assert.AreEqual(exp, Convert.ToInt32(doc["x"]));
                exp--;
            }            
        }
        
        [Test]
        public void TestExplain(){
            Document exp = db["tests"]["reads"].FindAll().Limit(5).Skip(5).Sort("x").Explain();
            Assert.IsTrue(exp.Contains("cursor"));
            Assert.IsTrue(exp.Contains("n"));
            Assert.IsTrue(exp.Contains("nscanned"));
        }
        
        [Test]
        public void TestHint(){
            IMongoCollection reads = db["tests"]["reads"];
            Document hint = new Document().Append("x",IndexOrder.Ascending);
            
            Document exp = reads.FindAll().Hint(hint).Explain();
            Assert.IsTrue(exp.Contains("$err"), "No error found");

            reads.MetaData.CreateIndex("hintindex",hint,false);
            exp = reads.FindAll().Hint(hint).Explain();
            
            Assert.IsTrue(exp.Contains("cursor"));
            Assert.IsTrue(exp.Contains("n"));
            Assert.IsTrue(exp.Contains("nscanned"));            
        }
        
        [TestFixtureSetUp]
        public void Init(){
            db.Connect();
            cleanDB();
        }

        [TestFixtureTearDown]
        public void Dispose(){
            db.Disconnect();
        }
        
        protected void cleanDB(){
            try{
                db["tests"].MetaData.DropCollection("sorts");
            }catch(Exception){}
            try{
                db["tests"]["reads"].MetaData.DropIndex("hintindex");
            }catch(Exception){}
                   
        }
    }
}
