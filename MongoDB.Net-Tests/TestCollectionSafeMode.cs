using System;

using NUnit.Framework;


namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollectionSafeMode
    {
                public Mongo Mongo{get;set;}
        public Database DB{
            get{
                return this.Mongo["tests"];
            }
        }
        public string TestCollections {
        //public override string TestCollections {            
            get {
                return "safeinsert, safeupdate, safedelete";
            }
        }
        
        
   
        [Test]
        public void TestBadInsert(){
            IMongoCollection col = DB["safeinsert"];
            col.MetaData.CreateIndex(new Document(){{"x", IndexOrder.Ascending}}, true);
            Document dup = new Document(){{"x",1},{"y",2}};
            col.Insert(dup);
            DB.ResetError();
            
            bool thrown = false;
            try{
                col.Insert(dup,true);
            }catch(MongoDuplicateKeyException mdk){
                Console.WriteLine(mdk.Message);
                Console.WriteLine(mdk.Error);
                thrown = true;
            }catch(Exception e){
                Assert.Fail(String.Format("Wrong exception thrown: {0}" + e.GetType().Name));
            }
            Assert.IsTrue(thrown);
        }
        
        [TestFixtureSetUp]
        public void Init(){
            //base.Init();
        }
        
        [TestFixtureTearDown]
        public void Dispose(){
            //base.Dispose();
        }
    }
}
