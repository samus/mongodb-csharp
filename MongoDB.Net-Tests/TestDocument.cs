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
        public void TestCopyToCopiesAndPreservesKeyOrderToExistingDoc(){
            Document d = new Document();
            Document dest = new Document();
            dest["two"] = 200;
            d["one"] = 1;
            d.Add("two", 2);
            d["three"] = 3;
            d.CopyTo(dest);
            int cnt = 1;
            foreach(String key in dest.Keys){
                Assert.AreEqual(cnt, d[key], "Order wasn't reset on CopyTo");
                cnt++;
            }           
        }       
    }
}
