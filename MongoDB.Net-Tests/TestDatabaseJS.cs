using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace MongoDB.Driver{


    [TestFixture()]
    public class TestDatabaseJS
    {
        Mongo db = new Mongo();
        Database tests;
        DatabaseJS js;
        
        [Test()]
        public void TestCanGetDatabaseJSObject(){
            Assert.IsNotNull(tests.JS);
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
        
        
        [TestFixtureSetUp]
        public void Init(){
            db.Connect();
            initDB();
            tests = db["tests"];
            js = tests.JS;
        }
        
        [TestFixtureTearDown]
        public void Dispose(){
            db.Disconnect();
        }
        
        protected void initDB(){
            //drop any previously created collections.
            db["tests"]["system.js"].Delete(new Document());
        }        
        
        protected void AddFunction(string name){
            Code func = new Code("function(x,y){return x + y;}");
            tests["system.js"].Insert(new Document().Append("_id", name).Append("value", func));
        }
    }
}
