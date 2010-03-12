using System;

using NUnit.Framework;


namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollectionSafeMode : MongoTestBase
    {
        public override string TestCollections {            
            get {
                return "safeinsert, safeupdate, safedelete, safemupdate";
            }
        }
        
        
   
        [Test]
        public void TestBadInsert(){
            IMongoCollection<Document> col = InitCollection("safeinsert");
            bool thrown = false;
            try{
                col.Insert(new Document {{"x",1},{"y",2}},true);
            }catch(MongoDuplicateKeyException){
                thrown = true;
            }catch(Exception e){
                Assert.Fail(String.Format("Wrong exception thrown: {0}", e.GetType().Name));
            }
            Assert.IsTrue(thrown);
        }
        
        [Test]
        public void TestBadUpdate(){
            IMongoCollection<Document> col = InitCollection("safeupdate");
            bool thrown = false;
            try{
                col.Update(new Document {{"x", 1}}, new Document{{"x",2}},true);
            }catch(MongoDuplicateKeyUpdateException){
                thrown = true;
            }catch(MongoDuplicateKeyException){
                Assert.Fail("MongoDuplicateKeyException thown instead of MongoDuplicateKeyUpdateException");
            }catch(Exception e){
                
                Assert.Fail(String.Format("Wrong exception thrown: {0}", e.GetType().Name));
            }
            Assert.IsTrue(thrown);
        }
        
        [Test]
        public void TestMultiUpdate(){
            IMongoCollection<Document> col = InitCollection("safemupdate");
            Document newy = new Document(){{"y", 2}};
            col.UpdateAll(newy, new Document(){{"y",1}},true);
            Assert.AreEqual(5, col.Count(newy));
            
            bool thrown = false;
            try{
                col.UpdateAll(new Document{{"x",1}}, new Document{{"y",2}},true);
            }catch(MongoDuplicateKeyUpdateException){
                thrown = true;
            }catch(MongoDuplicateKeyException){
                Assert.Fail("MongoDuplicateKeyException thown instead of MongoDuplicateKeyUpdateException");
            }catch(Exception e){
                
                Assert.Fail(String.Format("Wrong exception thrown: {0}", e.GetType().Name));
            }
            Assert.IsTrue(thrown, "Exception not thrown.");            
        }

        protected IMongoCollection<Document> InitCollection(string name)
        {
            IMongoCollection<Document> col = DB[name];
            col.MetaData.CreateIndex(new Document{{"x", IndexOrder.Ascending}}, true);
            for(int x = 0; x < 5; x++){
                col.Insert(new Document{{"x", x}, {"y", 1}});
            }
            return col;
        }
    }
}
