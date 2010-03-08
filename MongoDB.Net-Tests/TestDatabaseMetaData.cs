using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestDatabaseMetaData : MongoTestBase
    {
        public override string TestCollections {
            get {
                return "creatednoopts,createdcapped,createdinvalid";
            }
        }

        public override void OnInit () {
            //Add any new collections ones to work on.
            DB["$cmd"].FindOne(new Document().Append("create","todrop"));
        }       
        
        [Test]
        public void TestCreateCollectionNoOptions(){
            DB.MetaData.CreateCollection("creatednoopts");
            
            List<String> names = DB.GetCollectionNames();
            Assert.IsTrue(names.Contains("tests.creatednoopts"));
            
        }
        
        [Test]
        public void TestCreateCollectionWithOptions(){
            Document options = new Document().Append("capped",true).Append("size",10000);
            DB.MetaData.CreateCollection("createdcapped",options);           

            List<String> names = DB.GetCollectionNames();
            Assert.IsTrue(names.Contains("tests.createdcapped"));

        }

        [Test]
        public void TestCreateCollectionWithInvalidOptions(){
            Document options = new Document().Append("invalidoption",true);
            DB.MetaData.CreateCollection("createdinvalid",options);          

            List<String> names = DB.GetCollectionNames();
            Assert.IsTrue(names.Contains("tests.createdinvalid"));

        }
        
        [Test]
        public void TestDropCollection(){
            bool dropped = DB.MetaData.DropCollection("todrop");
            
            Assert.IsTrue(dropped,"Dropped was false");

            List<String> names = DB.GetCollectionNames();
            Assert.IsFalse(names.Contains("tests.todrop"));
            
        }
        
        [Test]
        public void TestDropInvalidCollection(){
            bool thrown = false;
            try{
                DB.MetaData.DropCollection("todrop_notexists");
            }catch(MongoCommandException){
                thrown = true;
            }
            
            Assert.IsTrue(thrown,"Command exception should have been thrown");
            
            List<String> names = DB.GetCollectionNames();
            Assert.IsFalse(names.Contains("tests.todrop_notexists"));
            
        }       
        
        
    }   
}
