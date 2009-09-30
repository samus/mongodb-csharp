

using System;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestMongoCommException
    {
        [Test]
        public void TestThrow(){
            try{
                
            }catch(MongoCommException mce){
                Assert.AreEqual("localhost", mce.Host);
            }
        }
    }
}
