/*
 * User: scorder
 * Date: 7/8/2009
 */

using System;
using System.Collections;

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
            ICollection vals = d.Values;
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
            Document d1 = new Document().Append("k1", "v1").Append("k2", "v2");
            Document d2 = new Document().Append("k1", "v1").Append("k2", "v2");
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameContentInDifferentOrderAreNotEqual() {
            Document d1 = new Document().Append("k1", "v1").Append("k2", "v2");
            Document d2 = new Document().Append("k2", "v2").Append("k1", "v1");
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameArrayContentAreEqual() {
            Document d1 = new Document().Append("k1", new string[] { "v1", "v2" });
            Document d2 = new Document().Append("k1", new string[] { "v1", "v2" });
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithMisorderedArrayContentAreNotEqual() {
            Document d1 = new Document().Append("k1", new string[] { "v1", "v2" });
            Document d2 = new Document().Append("k1", new string[] { "v2", "v1" });
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithSameDocumentChildTreeAreEqual() {
            Document d1 = new Document().Append("k1", new Document().Append("k2",new Document().Append("k3","foo")));
            Document d2 = new Document().Append("k1", new Document().Append("k2", new Document().Append("k3", "foo")));
            AreEqual(d1, d2);
        }

        [Test]
        public void TestTwoDocumentsWithDifferentDocumentChildTreeAreNotEqual() {
            Document d1 = new Document().Append("k1", new Document().Append("k2", new Document().Append("k3", "foo")));
            Document d2 = new Document().Append("k1", new Document().Append("k2", new Document().Append("k3", "bar")));
            AreNotEqual(d1, d2);
        }

        [Test]
        public void TestToStringForDocWithSingleNullField() {
            var doc = new Document().Append("foo", null);
            Assert.AreEqual(@"{ ""foo"": null }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSingleTrueField() {
            var doc = new Document().Append("foo", true);
            Assert.AreEqual(@"{ ""foo"": true }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSingleFalseField() {
            var doc = new Document().Append("foo", false);
            Assert.AreEqual(@"{ ""foo"": false }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSingleStringField() {
            var doc = new Document().Append("foo", "bar");
            Assert.AreEqual(@"{ ""foo"": ""bar"" }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSingleIntField() {
            var doc = new Document().Append("foo", 10);
            Assert.AreEqual(@"{ ""foo"": 10 }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSingleDoubleField() {
            var doc = new Document().Append("foo", 10.1);
            Assert.AreEqual(@"{ ""foo"": 10.1 }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSingleDateTimeField() {
            var doc = new Document().Append("foo", DateTime.Parse("2009-10-10T07:00:00.0000000Z"));
            Assert.AreEqual(@"{ ""foo"": ""2009-10-10T07:00:00.0000000Z"" }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSingleOidField() {
            var doc = new Document().Append("foo", new Oid("4ac7ee91f693066f1c96b649"));
            Assert.AreEqual(@"{ ""foo"": ""4ac7ee91f693066f1c96b649"" }", doc.ToString());
        }
        [Test]
        public void TestToStringForDocWithMultipleFields() {
            var doc = new Document().Append("foo", "bar").Append("baz", 42);
            Assert.AreEqual(@"{ ""foo"": ""bar"", ""baz"": 42 }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithSubDocField() {
            var doc = new Document().Append("foo", "bar").Append("baz", new Document().Append("a", 1));
            Assert.AreEqual(@"{ ""foo"": ""bar"", ""baz"": { ""a"": 1 } }", doc.ToString());
        }
        [Test]
        public void TestToStringForDocWithArrayOfInts() {
            var doc = new Document().Append("foo", new[] {1,2,3,4});
            Assert.AreEqual(@"{ ""foo"": [ 1, 2, 3, 4 ] }", doc.ToString());
        }

        [Test]
        public void TestToStringForDocWithArrayOfDocs() {
            var doc = new Document().Append("foo", new[] {
                new Document().Append("a", 1),
                new Document().Append("b", 2),
                new Document().Append("c", 3),
            });
            Assert.AreEqual(@"{ ""foo"": [ { ""a"": 1 }, { ""b"": 2 }, { ""c"": 3 } ] }", doc.ToString());
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
