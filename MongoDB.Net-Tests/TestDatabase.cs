using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestDatabase
    {
        Mongo mongo = new Mongo();
        Database db;
        
        [Test]
        public void TestFollowReference(){
            Database tests = mongo["tests"];
            Oid id = new Oid("4a7067c30a57000000008ecb");
            DBRef rf = new DBRef("reads", id);
            
            Document target = tests.FollowReference(rf);
            Assert.IsNotNull(target, "FollowReference returned null");
            Assert.IsTrue(target.Contains("j"));
            Assert.AreEqual((double)9980, (double)target["j"]);
        }
        
        [Test]
        public void TestFollowNonReference(){
            Database tests = mongo["tests"];
            Oid id = new Oid("BAD067c30a57000000008ecb");
            DBRef rf = new DBRef("reads", id);
            
            Document target = tests.FollowReference(rf);
            Assert.IsNull(target, "FollowReference returned wasn't null");          
        }
        
        [Test]
        public void TestReferenceNonOid(){
            Database tests = mongo["tests"];
            IMongoCollection refs = tests["refs"];
            
            Document doc = new Document().Append("_id",123).Append("msg", "this has a non oid key");
            refs.Insert(doc);
            
            DBRef rf = new DBRef("refs",123);
            
            Document recv = tests.FollowReference(rf);
            
            Assert.IsNotNull(recv);
            Assert.IsTrue(recv.Contains("msg"));
            Assert.AreEqual(recv["_id"], (long)123);
        }
        
        [Test]
        public void TestGetCollectionNames(){
            List<String> names = mongo["tests"].GetCollectionNames();
            Assert.IsNotNull(names,"No collection names returned");
            Assert.IsTrue(names.Count > 0);
            Assert.IsTrue(names.Contains("tests.inserts"));
        }
        
        [Test]
        public void TestEvalNoScope(){
            Document result = db.Eval("function(){return 3;}");
            Assert.AreEqual(3, result["retval"]);
        }
        
        [Test]
        public void TestEvalWithScope(){
            int val = 3;
            Document scope = new Document().Append("x",val);
            Document result = db.Eval("function(){return x;}", scope);
            Assert.AreEqual(val, result["retval"]);            
        }
        
        [Test]
        public void TestEvalWithScopeAsFunctionParameters(){
            int x = 3;
            int y = 4;
            string func = "adder = function(a, b){return a + b;}; return adder(x,y)";
            Document scope = new Document().Append("x",x).Append("y", y);
            Document result = db.Eval(func, scope);
            Console.Out.WriteLine(result.ToString());
            Assert.AreEqual(x + y, result["retval"]);                        
        }
       
        [Test]
        public void TestGetLastErrorNoError(){
            db["noerror"].Insert(new Document(){{"a",1},{"b",2}});
            Document error = db.GetLastError();
            Assert.AreEqual(DBNull.Value, error["err"]);
        }
        
        [Test]
        public void TestGetLastError(){
            IMongoCollection errcol = db["errcol"];
            errcol.MetaData.CreateIndex(new Document(){{"x", IndexOrder.Ascending}}, true);
            Document dup = new Document(){{"x",1},{"y",2}};
            errcol.Insert(dup);
            Document error = db.GetLastError();
            Assert.AreEqual(DBNull.Value, error["err"]);
            
            errcol.Insert(dup);
            error = db.GetLastError();

            Assert.IsFalse(DBNull.Value == error["err"]);
            
        }
        
        [Test]
        public void TestGetPrevError(){
            IMongoCollection col = db["preverror"];
            col.MetaData.CreateIndex(new Document(){{"x", IndexOrder.Ascending}},true);
            List<Document> docs = new List<Document>();
            for(int x = 0; x < 10; x++){
                docs.Add(new Document(){{"x",x},{"y",2}});
            }
            docs.Add(new Document(){{"x",1},{"y",4}}); //the dupe
            db.ResetError();
            Assert.AreEqual(DBNull.Value, db.GetLastError()["err"]);
            
            col.Insert(docs);
            Document error = db.GetLastError();
            
            Assert.IsFalse(DBNull.Value == error["err"]);
            Console.WriteLine(error);
        }

        [TestFixtureSetUp]
        public void Init(){
            mongo.Connect();
            db = mongo["tests"];
            cleanDB();
        }
        
        [TestFixtureTearDown]
        public void Dispose(){
            mongo.Disconnect();
        }
        
        protected void cleanDB(){
            mongo["tests"]["$cmd"].FindOne(new Document().Append("drop","refs"));
            mongo["tests"]["$cmd"].FindOne(new Document().Append("drop","noerror"));
            mongo["tests"]["$cmd"].FindOne(new Document().Append("drop","errcol"));
            mongo["tests"]["$cmd"].FindOne(new Document().Append("drop","preverror"));
        }
    }
}
