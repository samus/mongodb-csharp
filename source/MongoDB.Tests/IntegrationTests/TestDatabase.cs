using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.IntegrationTests
{
    [TestFixture]
    public class TestDatabase : MongoTestBase
    {
        public override string TestCollections{
            get { return "refs,noerror,errcol,preverror"; }
        }

        [Test]
        public void TestEvalNoScope(){
            var result = TestsDatabase.Eval("function(){return 3;}");
            Assert.AreEqual(3, result["retval"]);
        }

        [Test]
        public void TestEvalWithScope(){
            var val = 3;
            var scope = new Document().Add("x", val);
            var result = TestsDatabase.Eval("function(){return x;}", scope);
            Assert.AreEqual(val, result["retval"]);
        }

        [Test]
        public void TestEvalWithScopeAsFunctionParameters(){
            var x = 3;
            var y = 4;
            var func = "adder = function(a, b){return a + b;}; return adder(x,y)";
            var scope = new Document().Add("x", x).Add("y", y);
            var result = TestsDatabase.Eval(func, scope);
            Console.Out.WriteLine(result.ToString());
            Assert.AreEqual(x + y, result["retval"]);
        }

        [Test]
        public void TestFollowNonReference(){
            var id = new Oid("BAD067c30a57000000008ecb");
            var rf = new DBRef("refs", id);

            var target = TestsDatabase.FollowReference(rf);
            Assert.IsNull(target, "FollowReference returned wasn't null");
        }

        [Test]
        public void TestFollowReference(){
            var refs = TestsDatabase["refs"];
            var id = new Oid("4a7067c30a57000000008ecb");
            var msg = "this has an oid key";
            var doc = new Document {{"_id", id}, {"msg", msg}};
            refs.Insert(doc);

            var rf = new DBRef("refs", id);

            var target = TestsDatabase.FollowReference(rf);
            Assert.IsNotNull(target, "FollowReference returned null");
            Assert.IsTrue(target.Contains("msg"));
            Assert.AreEqual(msg, target["msg"]);
        }

        [Test]
        public void TestGetCollectionNames(){
            var names = TestsDatabase.GetCollectionNames();
            Assert.IsNotNull(names, "No collection names returned");
            Assert.IsTrue(names.Count > 0);
            Assert.IsTrue(names.Contains("tests.inserts"));
        }

        [Test]
        public void TestGetLastError(){
            var errcol = TestsDatabase["errcol"];
            errcol.MetaData.CreateIndex(new Document {{"x", IndexOrder.Ascending}}, true);
            var dup = new Document {{"x", 1}, {"y", 2}};
            errcol.Insert(dup);
            var error = TestsDatabase.GetLastError();
            Assert.AreEqual(null, error["err"]);

            errcol.Insert(dup);
            error = TestsDatabase.GetLastError();

            Assert.IsFalse(null == error["err"]);
        }

        [Test]
        public void TestGetLastErrorNoError(){
            TestsDatabase["noerror"].Insert(new Document {{"a", 1}, {"b", 2}});
            var error = TestsDatabase.GetLastError();
            Assert.AreEqual(null, error["err"]);
        }

        [Test]
        public void TestGetPrevError(){
            var col = TestsDatabase["preverror"];
            col.MetaData.CreateIndex(new Document {{"x", IndexOrder.Ascending}}, true);
            var docs = new List<Document>();
            for(var x = 0; x < 10; x++)
                docs.Add(new Document {{"x", x}, {"y", 2}});
            docs.Add(new Document {{"x", 1}, {"y", 4}}); //the dupe
            TestsDatabase.ResetError();
            Assert.AreEqual(null, TestsDatabase.GetLastError()["err"]);

            col.Insert(docs);
            var error = TestsDatabase.GetLastError();

            Assert.IsFalse(null == error["err"]);
        }

        [Test]
        public void TestReferenceNonOid(){
            var refs = TestsDatabase["refs"];

            var doc = new Document().Add("_id", 123).Add("msg", "this has a non oid key");
            refs.Insert(doc);

            var rf = new DBRef("refs", 123);

            var recv = TestsDatabase.FollowReference(rf);

            Assert.IsNotNull(recv);
            Assert.IsTrue(recv.Contains("msg"));
            Assert.AreEqual(recv["_id"], (long)123);
        }

        [Test]
        public void TestCanCreateFromConnectionString(){
            var builder = new MongoConnectionStringBuilder(ConnectionString) {Database = "tests"};
            /*
            using(var database = new MongoDatabase(builder.ToString()))
            {
                
            }*/
        }
    }
}