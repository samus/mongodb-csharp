using MongoDB.Driver.Connections;
using NUnit.Framework;

namespace MongoDB.Driver.IntegrationTests.Connections
{
    [TestFixture]
    public class TestSimpleConnectionFactory
    {
        [Test]
        public void TestCanOpenConnection(){
            using(var simpleConnectionFactory = new SimpleConnectionFactory(string.Empty))
            using(var connection = simpleConnectionFactory.Open()){
                Assert.IsNotNull(connection);
            }
        }

        public void TestCanCloseConnection(){
            using(var simpleConnectionFactory = new SimpleConnectionFactory(string.Empty))
            using(var connection = simpleConnectionFactory.Open())
            {
                simpleConnectionFactory.Close(connection);
                Assert.IsFalse(connection.IsConnected);
            }
        }
    }
}