using System;

using NUnit.Framework;


namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollectionSafeMode : MongoTestBase
    {
        public override string TestCollections {            
            get {
                return "safeinsert, safeupdate, safedelete";
            }
        }
        
        
   
        [Test]
        public void TestBadInsert(){
            IMongoCollection col = InitCollection("safeinsert");
            bool thrown = false;
            try{
                col.Insert(new Document(){{"x",1},{"y",2}},true);
            }catch(MongoDuplicateKeyException mdk){
                thrown = true;
            }catch(Exception e){
                Assert.Fail(String.Format("Wrong exception thrown: {0}", e.GetType().Name));
            }
            Assert.IsTrue(thrown);
        }
        
        [Test]
        public void TestBadUpdate(){
            IMongoCollection col = InitCollection("safeupdate");
            bool thrown = false;
            try{
                col.Update(new Document(){{"x", 1}}, new Document(){{"x",2}},true);
            }catch(MongoDuplicateKeyUpdateException){
                thrown = true;
            }catch(MongoDuplicateKeyException mdk){
                Console.WriteLine(mdk.Error);
                Assert.Fail("MongoDuplicateKeyException thown instead of MongoDuplicateKeyUpdateException");
            }catch(Exception e){
                
                Assert.Fail(String.Format("Wrong exception thrown: {0}", e.GetType().Name));
            }
            Assert.IsTrue(thrown);
        }
        
        
        protected IMongoCollection InitCollection(string name){
            IMongoCollection col = DB[name];
            col.MetaData.CreateIndex(new Document(){{"x", IndexOrder.Ascending}}, true);
            for(int x = 0; x < 5; x++){
                col.Insert(new Document(){{"x", x}, {"y", 1}});
            }
            return col;
        }
    }
}
