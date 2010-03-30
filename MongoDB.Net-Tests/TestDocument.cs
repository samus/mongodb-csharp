/*
 * User: scorder
 * Date: 7/8/2009
 */

using System;
using System.Linq;
using System.Collections;

using NUnit.Framework;

using MongoDB.Driver;
using System.Collections.Generic;

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
        public void TestUseOfIComparerForKeys()
        {
            var doc = new Document(new ReverseComparer());

            doc.Append("a", 3);
            doc.Append("b", 2);
            doc.Append("c", 1);

            Assert.AreEqual("c", doc.Keys.First());
        }

        [Test]
        public void TestInsertMaintainsKeyOrder()
        {
            Document d = new Document();
            d["one"] = 1;
            d.Insert("zero", 0, 0);

            var keysList = d.Keys as IEnumerable<string>;
            Assert.AreEqual(keysList.First(), "zero");
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(ArgumentException), 
            ExpectedMessage="Key already exists in Document",
            MatchType=MessageMatch.Contains)]
        public void TestInsertWillThrowArgumentExceptionIfKeyAlreadyExists()
        {
            Document d = new Document();
            d["one"] = 1;
            d.Insert("one", 1, 0);
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

    public class ReverseComparer : IComparer<string>
    {

        public int Compare(string x, string y)
        {
            return y.CompareTo(x);
        }
    }
}
