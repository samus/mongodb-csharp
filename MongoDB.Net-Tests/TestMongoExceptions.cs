using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestMongoCommException
    {
        [Test]
        public void TestThrow(){
            try{
                
            }catch(MongoCommException){
                //Assert.AreEqual("localhost", mce.ConnectionString);
            }
        }
    }
}
