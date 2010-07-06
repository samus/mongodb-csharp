using System;
using MongoDB.Connections;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.Connections
{
    [TestFixture]
    public class TestConnectionFactory
    {
        [TearDown]
        public void TearDown (){
            ConnectionFactoryFactory.Shutdown ();
        }

        [Test]
        public void TestGetConnection (){
            var connection1 = ConnectionFactoryFactory.GetConnection (string.Empty);
            var connection2 = ConnectionFactoryFactory.GetConnection (string.Empty);
            Assert.IsNotNull (connection1);
            Assert.IsNotNull (connection2);
            Assert.AreEqual (1, ConnectionFactoryFactory.PoolCount);
        }

        [Test]
        public void TestCreatePoolForEachUniqeConnectionString (){
            ConnectionFactoryFactory.GetConnection (string.Empty);
            ConnectionFactoryFactory.GetConnection (string.Empty);
            ConnectionFactoryFactory.GetConnection ("Username=test");
            ConnectionFactoryFactory.GetConnection ("Username=test");
            ConnectionFactoryFactory.GetConnection ("Server=localhost");
            Assert.AreEqual (3, ConnectionFactoryFactory.PoolCount);
        }
        
        [Test]
        public void TestExceptionWhenMinimumPoolSizeIsGreaterThenMaximumPoolSize (){
            try{
                ConnectionFactoryFactory.GetConnection("MinimumPoolSize=50; MaximumPoolSize=10");
            }catch(ArgumentException){
            }catch(Exception){
                Assert.Fail("Wrong exception thrown");
            }
        }
    }
}
