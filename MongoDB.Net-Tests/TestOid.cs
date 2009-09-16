

using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestOid
    {
        [Test]
        public void TestIDLength()
        {
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
                new Oid(null);
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
                System.Console.WriteLine(ae.ToString());
            }
            Assert.IsFalse(thrown,"ID should be fine.");                        
        }
    }
}
