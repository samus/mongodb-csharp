using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestDatabase : MongoTestBase
    {
        public override string TestCollections {
            get {
                return "refs,noerror,errcol,preverror";
            }
        }

        [Test]
        public void TestFollowReference(){
            IMongoCollection<Document> refs = DB["refs"];
            Oid id = new Oid("4a7067c30a57000000008ecb");
            string msg = "this has an oid key";
            Document doc = new Document(){{"_id", id},{"msg", msg}};
            refs.Insert(doc);
            
            DBRef rf = new DBRef("refs", id);
            
            Document target = DB.FollowReference(rf);
            Assert.IsNotNull(target, "FollowReference returned null");
            Assert.IsTrue(target.Contains("msg"));
            Assert.AreEqual(msg, target["msg"]);
        }
        
        [Test]
        public void TestFollowNonReference(){
            Oid id = new Oid("BAD067c30a57000000008ecb");
            DBRef rf = new DBRef("refs", id);
            
            Document target = DB.FollowReference(rf);
            Assert.IsNull(target, "FollowReference returned wasn't null");          
        }
        
        [Test]
        public void TestReferenceNonOid(){
            IMongoCollection<Document> refs = DB["refs"];
            
            Document doc = new Document().Add("_id",123).Add("msg", "this has a non oid key");
            refs.Insert(doc);
            
            DBRef rf = new DBRef("refs",123);
            
            Document recv = DB.FollowReference(rf);
            
            Assert.IsNotNull(recv);
            Assert.IsTrue(recv.Contains("msg"));
            Assert.AreEqual(recv["_id"], (long)123);
        }
        
        [Test]
        public void TestGetCollectionNames(){
            List<String> names = DB.GetCollectionNames();
            Assert.IsNotNull(names,"No collection names returned");
            Assert.IsTrue(names.Count > 0);
            Assert.IsTrue(names.Contains("tests.inserts"));
        }
        
        [Test]
        public void TestEvalNoScope(){
            Document result = DB.Eval("function(){return 3;}");
            Assert.AreEqual(3, result["retval"]);
        }
        
        [Test]
        public void TestEvalWithScope(){
            int val = 3;
            Document scope = new Document().Add("x",val);
            Document result = DB.Eval("function(){return x;}", scope);
            Assert.AreEqual(val, result["retval"]);            
        }
        
        [Test]
        public void TestEvalWithScopeAsFunctionParameters(){
            int x = 3;
            int y = 4;
            string func = "adder = function(a, b){return a + b;}; return adder(x,y)";
            Document scope = new Document().Add("x",x).Add("y", y);
            Document result = DB.Eval(func, scope);
            Console.Out.WriteLine(result.ToString());
            Assert.AreEqual(x + y, result["retval"]);                        
        }
       
        [Test]
        public void TestGetLastErrorNoError(){
            DB["noerror"].Insert(new Document(){{"a",1},{"b",2}});
            Document error = DB.GetLastError();
            Assert.AreEqual(DBNull.Value, error["err"]);
        }
        
        [Test]
        public void TestGetLastError(){
            IMongoCollection<Document> errcol = DB["errcol"];
            errcol.MetaData.CreateIndex(new Document(){{"x", IndexOrder.Ascending}}, true);
            Document dup = new Document(){{"x",1},{"y",2}};
            errcol.Insert(dup);
            Document error = DB.GetLastError();
            Assert.AreEqual(DBNull.Value, error["err"]);
            
            errcol.Insert(dup);
            error = DB.GetLastError();

            Assert.IsFalse(DBNull.Value == error["err"]);
            
        }
        
        [Test]
        public void TestGetPrevError(){
            IMongoCollection<Document> col = DB["preverror"];
            col.MetaData.CreateIndex(new Document(){{"x", IndexOrder.Ascending}},true);
            List<Document> docs = new List<Document>();
            for(int x = 0; x < 10; x++){
                docs.Add(new Document(){{"x",x},{"y",2}});
            }
            docs.Add(new Document(){{"x",1},{"y",4}}); //the dupe
            DB.ResetError();
            Assert.AreEqual(DBNull.Value, DB.GetLastError()["err"]);
            
            col.Insert(docs);
            Document error = DB.GetLastError();
            
            Assert.IsFalse(DBNull.Value == error["err"]);

        }
    }
}
