/*
 * User: scorder
 * Date: 7/8/2009
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

using MongoDB.Driver;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestDocument
    {
        [Test]
        public void TestValuesAdded()
        {
            Document d = new Document();
            d["test"] = 1;
            Assert.AreEqual(1, d["test"]);
        }
        
        [Test]
        public void TestKeyOrderIsPreserved(){
            Document d = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            int cnt = 1;
            foreach(String key in d.Keys){
                Assert.AreEqual(cnt, d[key]);
                cnt++;
            }
        }
        [Test] 
        public void TestRemove(){
            Document d = new Document();
            d["one"] = 1;
            d.Remove("one");
            Assert.IsFalse(d.Contains("one"));
        }
        
        [Test]
        public void TestKeyOrderPreservedOnRemove(){
            Document d = new Document();
            d["one"] = 1;
            d["onepointfive"] = 1.5;
            d.Add("two", 2);
            d.Add("two.5", 2.5);
            d.Remove("two.5");
            d["three"] = 3;
            d.Remove("onepointfive");
            int cnt = 1;
            foreach(String key in d.Keys){
                Assert.AreEqual(cnt, d[key]);
                cnt++;
            }
        }
        
        [Test]
        public void TestValues(){
            Document d = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            var vals = d.Values;
            Assert.AreEqual(3, vals.Count);

        }
        
        [Test]
        public void TestClearRemovesAll(){
            Document d = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            Assert.AreEqual(3,d.Count);
            d.Clear();
            Assert.AreEqual(0, d.Count);
            Assert.IsNull(d["one"]);
            Assert.IsFalse(d.Contains("one"));
        }
        
        [Test]
        public void TestCopyToCopiesAndPreservesKeyOrderToEmptyDoc(){
            Document d = new Document();
            Document dest = new Document();
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            d.CopyTo(dest);
            int cnt = 1;
            foreach(String key in dest.Keys){
                Assert.AreEqual(cnt, d[key]);
                cnt++;
            }           
        }
        
        [Test]
        public void TestCopyToCopiesAndOverwritesKeys(){
            Document d = new Document();
            Document dest = new Document();
            dest["two"] = 200;
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            d.CopyTo(dest);
            Assert.AreEqual(2, dest["two"]);
        }

        [Test]
        public void TestTwoDocumentsWithSameContentInSameOrderAreEqual() {
            Document d1 = new Document().Add("k1", "v1").Add("k2", "v2");
            Document d2 = new Document().Add("k1", "v1").Add("k2", "v2");
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameContentInDifferentOrderAreNotEqual() {
            Document d1 = new Document().Add("k1", "v1").Add("k2", "v2");
            Document d2 = new Document().Add("k2", "v2").Add("k1", "v1");
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameArrayContentAreEqual() {
            Document d1 = new Document().Add("k1", new string[] { "v1", "v2" });
            Document d2 = new Document().Add("k1", new string[] { "v1", "v2" });
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithMisorderedArrayContentAreNotEqual() {
            Document d1 = new Document().Add("k1", new string[] { "v1", "v2" });
            Document d2 = new Document().Add("k1", new string[] { "v2", "v1" });
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameDocumentChildTreeAreEqual() {
            Document d1 = new Document().Add("k1", new Document().Add("k2",new Document().Add("k3","foo")));
            Document d2 = new Document().Add("k1", new Document().Add("k2", new Document().Add("k3", "foo")));
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithDifferentDocumentChildTreeAreNotEqual() {
            Document d1 = new Document().Add("k1", new Document().Add("k2", new Document().Add("k3", "foo")));
            Document d2 = new Document().Add("k1", new Document().Add("k2", new Document().Add("k3", "bar")));
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestDocumentIsSerializable(){
            var src = new Document().Add("test", 2);
            using(var mem = new MemoryStream()){
                var formatter = new BinaryFormatter();
                formatter.Serialize(mem,src);
                mem.Position = 0;
                var dest = (Document)formatter.Deserialize(mem);
                AreEqual(src,dest);
            }
        }

        [Test]
        public void TestIdSets_IdField(){
            var document = new Document{Id = 10};
            Assert.AreEqual(10,document.Id);
        }

        [Test]
        public void TestIdReturnsNullIfNotSet(){
            var document = new Document();
            Assert.IsNull(document.Id);
        }

        [Test]
        public void TestDocumentCanCreatedFromDictionary(){
            var dictionary = new Dictionary<string, object>{{"value1", "test"}, {"value2", 10}};
            var document = new Document(dictionary);
            Assert.AreEqual(2,document.Count);
            Assert.AreEqual("test", document["value1"]);
            Assert.AreEqual(10, document["value2"]);
        }

        [Test]
        public void TestSetNullValue(){
            var document = new Document();
            document.Add("value", null);
            Assert.AreEqual(1,document.Count);
            Assert.IsNull(document["value"]);
        }

        private void AreEqual(Document d1, Document d2) {
            if (!d1.Equals(d2)) {
                Assert.Fail(string.Format("Documents don't match\r\nExpected: {0}\r\nActual:   {1}", d1, d2));
            }
        }
        
        private void AreNotEqual(Document d1, Document d2) {
            if (d1.Equals(d2)) {
                Assert.Fail(string.Format("Documents match\r\nExpected: not {0}\r\nActual:       {1}", d1, d2));
            }
        }
    }
}
