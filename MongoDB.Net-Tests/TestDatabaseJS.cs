using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace MongoDB.Driver{


    [TestFixture()]
    public class TestDatabaseJS : MongoTestBase
    {
        DatabaseJS js;
        public override string TestCollections {
            get {
                return "jsreads";
            }
        }
        
        public override void OnInit (){
            DB["system.js"].Delete(new Document());
            js = DB.JS;
            
            IMongoCollection jsreads = DB["jsreads"];
            for(int j = 1; j < 10; j++){
                jsreads.Insert(new Document(){{"j", j}});
            }
        }        
        
        [Test()]
        public void TestCanGetDatabaseJSObject(){
            Assert.IsNotNull(DB.JS);
        }
        
        [Test()]
        public void TestCanGetAFunction(){
            string name = "fget";
            AddFunction(name);
            Assert.IsNotNull(js[name]);
            Assert.IsNotNull(js.GetFunction(name));
        }
        
        [Test()]
        public void TestCanListFunctions(){
            string name = "flist";
            AddFunction(name);
            List<String> list = js.GetFunctionNames();
            Assert.IsTrue(list.Count > 0);
            
            bool found = false;
            foreach(string l in list){
                if(l == name) found = true;
            }
            Assert.IsTrue(found, "Didn't find the function that was inserted.");
        }
        
        [Test()]
        public void TestCanAddAFunctionStrStr(){
            string name = "faddss";
            string func = "function(x, y){return x + y;}";
            js.Add(name,func);
            Assert.IsNotNull(js[name]);
        }
        
        [Test()]
        public void TestCanAddAFunctionStrCode(){
            string name = "faddsc";
            Code func = new Code("function(x, y){return x + y;}");
            js.Add(name,func);
            Assert.IsNotNull(js[name]);            
        }
        
        [Test()]
        public void TestCanAddAFunctionDoc(){
            string name = "fadddoc";
            Code func = new Code("function(x, y){return x + y;}");
            Document doc = new Document().Append("_id", name).Append("value", func);
            js.Add(doc);
            Assert.IsNotNull(js[name]);                        
        }
        
        [Test]
        public void TestCannotAddAFunctionTwice(){
            string name = "faddtwice";
            Code func = new Code("function(x,y){return x + y;}");
            js.Add(name, func);
            bool thrown = false;
            try{
                js.Add(name, func);
            }catch(ArgumentException){
                thrown = true;
            }
            Assert.IsTrue(thrown, "Shouldn't be able to add a function twice");
        }
        
        [Test]
        public void TestCanAddFunctionByAssignment(){
            string name = "fassignadd";
            Code func = new Code("function(x,y){return x + y;}");
            Document doc = new Document().Append("_id", name).Append("value", func);
            js[name] = doc;
            Assert.IsNotNull(js[name]);
        }
        
        [Test]
        public void TestContains(){
            string name = "fcontains";
            AddFunction(name);
            Assert.IsTrue(js.Contains(name));
            Assert.IsFalse(js.Contains("none"));
            Assert.IsTrue(js.Contains(new Document().Append("_id", name).Append("value", new Code("dfs"))));
        }
        
        [Test]
        public void TestCopyTo(){
            int cnt = 5;
            Document[] funcs = new Document[cnt];
            Code func = new Code("function(x,y){return x +y;}");
            
            for(int i = 0; i < cnt; i++){
                string name = "_" + i + "fcopyTo";
                Document doc = new Document().Append("_id", name).Append("value", func);
                js[name] = doc;
            }
            
            js.CopyTo(funcs, 1);
            Assert.IsNull(funcs[0]);
            Assert.IsNotNull(funcs[1]);
            Assert.IsNotNull(funcs[4]);
            Assert.IsTrue(((string)funcs[1]["_id"]).StartsWith("_1")); //as long as no other _ named functions get in.
        }
        
        [Test]
        public void TestRemoveByName(){
            String name = "fremoven";
            AddFunction(name);
            Assert.IsTrue(js.Contains(name));
            js.Remove(name);
            Assert.IsFalse(js.Contains(name));
        }
        
        [Test]
        public void TestRemoveByDoc(){
            String name = "fremoved";
            Document func = new Document().Append("_id", name);
            AddFunction(name);
            Assert.IsTrue(js.Contains(name));
            js.Remove(func);
            Assert.IsFalse(js.Contains(name));
        }
        
        [Test]
        public void TestForEach(){
            string name = "foreach";
            AddFunction(name);
            bool found = false;
            foreach(Document doc in js){
                if(name.Equals(doc["_id"]))found = true;
            }
            Assert.IsTrue(found, "Added function wasn't found during foreach");
        }
        
        [Test]
        public void TestClear(){
            AddFunction("clear");
            Assert.IsTrue(js.Count > 0);
            js.Clear();
            Assert.IsTrue(js.Count == 0);
        }

        [Test]
        public void TestExec(){
            js.Add("lt4", new Code("function(doc){return doc.j < 4;}"));
            int cnt = 0;
            foreach(Document doc in DB["reads"].Find("lt4(this)").Documents){
                cnt++;
            }
            Assert.AreEqual(3,cnt);
        }
        
        [Test]
        public void TestExecWithScope(){
            js.Add("lt", new Code("function(doc){ return doc.j < limit;}"));
            int cnt = 0;
            Document scope = new Document().Append("limit", 5);
            Document query = new Document().Append("$where", new CodeWScope("lt(this)",scope));
            foreach(Document doc in DB["jsreads"].Find(query).Documents){
                cnt++;
            }
            Assert.AreEqual(4,cnt);
        }
        
        protected void AddFunction(string name){
            Code func = new Code("function(x,y){return x + y;}");
            DB["system.js"].Insert(new Document().Append("_id", name).Append("value", func));
        }
    }
}
