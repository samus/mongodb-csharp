

using System;

using NUnit.Framework;

namespace MongoDB.Driver
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
            }catch(ArgumentException ae){
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
    }
}
