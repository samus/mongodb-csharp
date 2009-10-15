using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestDatabaseMetaData
    {
        Mongo db = new Mongo();
        
        [Test]
        public void TestCreateCollectionNoOptions(){
            Database tests = db["tests"];
            tests.MetaData.CreateCollection("creatednoopts");
            
            List<String> names = tests.GetCollectionNames();
            Assert.IsTrue(names.Contains("tests.creatednoopts"));
            
        }
        
        [Test]
        public void TestCreateCollectionWithOptions(){
            Database tests = db["tests"];
            Document options = new Document().Append("capped",true).Append("size",10000);
            tests.MetaData.CreateCollection("createdcapped",options);           

            List<String> names = tests.GetCollectionNames();
            Assert.IsTrue(names.Contains("tests.createdcapped"));

        }

        [Test]
        public void TestCreateCollectionWithInvalidOptions(){
            Database tests = db["tests"];
            Document options = new Document().Append("invalidoption",true);
            tests.MetaData.CreateCollection("createdinvalid",options);          

            List<String> names = tests.GetCollectionNames();
            Assert.IsTrue(names.Contains("tests.createdinvalid"));

        }
        
        [Test]
        public void TestDropCollection(){
            Database tests = db["tests"];
            bool dropped = tests.MetaData.DropCollection("todrop");
            
            Assert.IsTrue(dropped,"Dropped was false");

            List<String> names = tests.GetCollectionNames();
            Assert.IsFalse(names.Contains("tests.todrop"));
            
        }
        
        [Test]
        public void TestDropInvalidCollection(){
            Database tests = db["tests"];
            bool thrown = false;
            try{
                tests.MetaData.DropCollection("todrop_notexists");
            }catch(MongoCommandException){
                thrown = true;
            }
            
            Assert.IsTrue(thrown,"Command exception should have been thrown");
            
            List<String> names = tests.GetCollectionNames();
            Assert.IsFalse(names.Contains("tests.todrop_notexists"));
            
        }       
        
        [TestFixtureSetUp]
        public void Init(){
            db.Connect();
            initDB();
        }
        
        [TestFixtureTearDown]
        public void Dispose(){
            db.Disconnect();
        }
        
        protected void initDB(){
            //drop any previously created collections.
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","creatednoopts"));
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","createdcapped"));
            db["tests"]["$cmd"].FindOne(new Document().Append("drop","createdinvalid"));
            
            //Add any new ones to work on.
            db["tests"]["$cmd"].FindOne(new Document().Append("create","todrop"));

                       
        }
    }   
}
