using System;
using System.Linq;
using NUnit.Framework;

namespace MongoDB.IntegrationTests
{
    [TestFixture]
    public class TestDatabaseJavascript : MongoTestBase
    {
        private DatabaseJavascript _javascript;

        public override string TestCollections
        {
            get { return "jsreads"; }
        }

        public override void OnInit()
        {
            DB["system.js"].Delete(new Document());
            _javascript = DB.Javascript;

            var jsreads = DB["jsreads"];
            for(var j = 1; j < 10; j++)
                jsreads.Insert(new Document {{"j", j}});
        }

        protected void AddFunction(string name)
        {
            var func = new Code("function(x,y){return x + y;}");
            DB["system.js"].Insert(new Document().Add("_id", name).Add("value", func));
        }

        [Test]
        public void TestCanAddAFunctionDoc()
        {
            const string name = "fadddoc";
            var func = new Code("function(x, y){return x + y;}");
            var doc = new Document().Add("_id", name).Add("value", func);
            _javascript.Add(doc);
            Assert.IsNotNull(_javascript[name]);
        }

        [Test]
        public void TestCanAddAFunctionStrCode()
        {
            const string name = "faddsc";
            var func = new Code("function(x, y){return x + y;}");
            _javascript.Add(name, func);
            Assert.IsNotNull(_javascript[name]);
        }

        [Test]
        public void TestCanAddAFunctionStrStr()
        {
            const string name = "faddss";
            var func = "function(x, y){return x + y;}";
            _javascript.Add(name, func);
            Assert.IsNotNull(_javascript[name]);
        }

        [Test]
        public void TestCanAddFunctionByAssignment()
        {
            const string name = "fassignadd";
            var func = new Code("function(x,y){return x + y;}");
            var doc = new Document().Add("_id", name).Add("value", func);
            _javascript[name] = doc;
            Assert.IsNotNull(_javascript[name]);
        }

        [Test]
        public void TestCanGetAFunction()
        {
            const string name = "fget";
            AddFunction(name);
            Assert.IsNotNull(_javascript[name]);
            Assert.IsNotNull(_javascript.GetFunction(name));
        }

        [Test]
        public void TestCanGetDatabaseJSObject()
        {
            Assert.IsNotNull(DB.Javascript);
        }

        [Test]
        public void TestCanListFunctions()
        {
            const string name = "flist";
            AddFunction(name);
            var list = _javascript.GetFunctionNames();
            Assert.IsTrue(list.Count > 0);

            var found = false;
            foreach(var l in list)
                if(l == name)
                    found = true;
            Assert.IsTrue(found, "Didn't find the function that was inserted.");
        }

        [Test]
        public void TestCannotAddAFunctionTwice()
        {
            const string name = "faddtwice";
            var func = new Code("function(x,y){return x + y;}");
            _javascript.Add(name, func);
            var thrown = false;
            try
            {
                _javascript.Add(name, func);
            }
            catch(ArgumentException)
            {
                thrown = true;
            }
            Assert.IsTrue(thrown, "Shouldn't be able to add a function twice");
        }

        [Test]
        public void TestClear()
        {
            AddFunction("clear");
            Assert.IsTrue(_javascript.Count > 0);
            _javascript.Clear();
            Assert.IsTrue(_javascript.Count == 0);
        }

        [Test]
        public void TestContains()
        {
            const string name = "fcontains";
            AddFunction(name);
            Assert.IsTrue(_javascript.Contains(name));
            Assert.IsFalse(_javascript.Contains("none"));
            Assert.IsTrue(_javascript.Contains(new Document().Add("_id", name).Add("value", new Code("dfs"))));
        }

        [Test]
        public void TestCopyTo()
        {
            const int cnt = 5;
            var funcs = new Document[cnt];
            var func = new Code("function(x,y){return x +y;}");

            for(var i = 0; i < cnt; i++)
            {
                var name = "_" + i + "fcopyTo";
                var doc = new Document().Add("_id", name).Add("value", func);
                _javascript[name] = doc;
            }

            _javascript.CopyTo(funcs, 1);
            Assert.IsNull(funcs[0]);
            Assert.IsNotNull(funcs[1]);
            Assert.IsNotNull(funcs[4]);

            Assert.AreEqual("_1fcopyTo", funcs[1]["_id"]);
            Assert.IsTrue(((string)funcs[1]["_id"]).StartsWith("_1")); //as long as no other _ named functions get in.
        }

        [Test]
        public void TestExec()
        {
            _javascript.Add("lt4", new Code("function(doc){return doc.j < 4;}"));
            var cnt = DB["reads"].Find("lt4(this)").Documents.Count();
            Assert.AreEqual(3, cnt);
        }

        [Test]
        public void TestExecWithScope()
        {
            _javascript.Add("lt", new Code("function(doc){ return doc.j < limit;}"));
            var scope = new Document().Add("limit", 5);
            var query = new Document().Add("$where", new CodeWScope("lt(this)", scope));
            var cnt = DB["jsreads"].Find(query).Documents.Count();
            Assert.AreEqual(4, cnt);
        }

        [Test]
        public void TestForEach()
        {
            var name = "foreach";
            AddFunction(name);
            var found = _javascript.Any(doc => name.Equals(doc["_id"]));
            Assert.IsTrue(found, "Added function wasn't found during foreach");
        }

        [Test]
        public void TestRemoveByDoc()
        {
            const string name = "fremoved";
            var func = new Document().Add("_id", name);
            AddFunction(name);
            Assert.IsTrue(_javascript.Contains(name));
            _javascript.Remove(func);
            Assert.IsFalse(_javascript.Contains(name));
        }

        [Test]
        public void TestRemoveByName()
        {
            const string name = "fremoven";
            AddFunction(name);
            Assert.IsTrue(_javascript.Contains(name));
            _javascript.Remove(name);
            Assert.IsFalse(_javascript.Contains(name));
        }
    }
}