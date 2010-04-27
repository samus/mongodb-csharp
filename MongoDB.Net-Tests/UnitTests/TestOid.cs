using System;
using NUnit.Framework;

namespace MongoDB.Driver.UnitTests
{
    [TestFixture]
    public class TestOid
    {
        [Test]
        public void TestIDLength(){
            bool thrown = false;
            try{
                new Oid("BAD0");
            }catch(Exception){
                thrown = true;
            }
            Assert.IsTrue(thrown,"No length exception thrown");
        }
        
        [Test]
        public void TestIDCharacters(){
            bool thrown = false;
            try{
                new Oid("BADBOYc30a57000000008ecb");
            }catch(Exception){
                thrown = true;
            }
            Assert.IsTrue(thrown,"No invalid characters exception thrown");         
        }
        
        [Test]
        public void TestQuoteCharacters(){
            Oid val = new Oid(@"""4a7067c30a57000000008ecb""");
            try{
                new Oid(val.ToString());
            }catch(ArgumentException){
                Assert.Fail("Creating an Oid from the json representation should not fail.");
            }
        }
        
        [Test]
        public void TestOidEquality(){
            Oid val = new Oid("4a7067c30a57000000008ecb");
            Oid other = new Oid("4a7067c30a57000000008ecb");
            
            Assert.IsTrue(val.Equals(other), "Equals(Oid) did not work");
            Assert.IsTrue(val == other, "== operator did not work");
            Assert.IsTrue(val == val, "Objects should be equal to itself.");
            
        }
                        
        [Test]
        public void TestOidInEquality(){
            Oid val = new Oid("4a7067c30a57000000008ecb");
            Oid other = new Oid("5a7067c30a57000000008ecb");
            Oid nilo = null;
            
            Assert.IsFalse(val == null);
            Assert.IsFalse(nilo == val);
            Assert.IsFalse(val == nilo);
            Assert.IsFalse(val == other);
            Assert.IsFalse(val.Equals(other));
            Assert.IsTrue(val != null);
            Assert.IsTrue(val != other);
            
        }
        
        [Test]
        public void TestOidEqualityToNull(){
            Oid val = Oid.NewOid();
            Oid other = null;
            Oid other2 = null;
            Assert.AreNotEqual(val, other);
            Assert.AreNotEqual(other, val);
            Assert.IsTrue(other == other2);
        }

        [Test]
        public void TestOidComparisons(){
            Oid lower = new Oid("4a7067c30a57000000008ecb");
            Oid higher = new Oid("5a7067c30a57000000008ecb");
            
            Assert.AreEqual(1, lower.CompareTo(null));
            Assert.AreEqual(1, higher.CompareTo(lower));
            
            Assert.IsTrue(lower < higher);
            Assert.IsTrue(higher > lower);
        }
        
        [Test]
        public void TestOidFromBytes(){
            byte[] bytes = new byte[]{1,2,3,4,5,6,7,8,9,10,11,12};
            string hex = "0102030405060708090a0b0c";
            
            Oid bval = new Oid(bytes);
            Oid sval = new Oid(hex);
            Assert.AreEqual(bval, sval);
        }
        
        [Test]
        public void TestNullValue(){
            bool thrown = false;
            try{
                new Oid(String.Empty);
            }catch(Exception){
                thrown = true;
            }
            Assert.IsTrue(thrown,"Null value exception not thrown");            
        }       
        
        [Test]
        public void TestCtor(){     
            bool thrown = false;
            try{
                new Oid("4a7067c30a57000000008ecb");
            }catch(ArgumentException){
                thrown = true;
            }
            Assert.IsFalse(thrown,"ID should be fine.");                        
        }
        
        [Test]
        public void TestDecode(){
            string hex = "4a7067c30a57000000008ecb";
            Oid oid = new Oid(hex);
            
            Assert.AreEqual("\"" + hex + "\"", oid.ToString());
        }
        
        [Test]
        public void TestEquals(){
            string hex = "4a7067c30a57000000008ecb";
            Assert.AreEqual(new Oid(hex), new Oid(hex));
            
        }
        [Test]
        public void TestNotEquals(){
            string hex = "4a7067c30a57000000008ecb";
            string hex2 = "4a7067c30a57000000008ecc";
            Assert.AreNotEqual(new Oid(hex), new Oid(hex2));
            
        }
        
        [Test]
        public void TestDate(){
            string hex = "4B458B95D114BE541B000000";
            Oid oid = new Oid(hex);
            //Expected: 2010-01-07 02:24:56.633
            DateTime expected = new DateTime(2010,1,7,7,21,57,DateTimeKind.Utc);
            Assert.AreEqual(expected,oid.Created);
        }
        
        [Test]
        public void TestToByteArray(){
            byte[] bytes = new byte[]{1,2,3,4,5,6,7,8,9,10,11,12};

            Oid bval = new Oid(bytes);
            byte[] bytes2 = bval.ToByteArray();
            
            Assert.IsNotNull(bytes2);
            Assert.AreEqual(12, bytes2.Length);
            Assert.AreEqual(bytes, bytes2);
        }        

        [Test]
        public void TestNewOidFromToString(){
            var hex = "4B458B95D114BE541B000000";
            var firstOid = new Oid(hex);
            var secondOid = new Oid(firstOid.ToString());
            
            Assert.AreEqual(firstOid.ToString(), secondOid.ToString());
        }

    }
}
