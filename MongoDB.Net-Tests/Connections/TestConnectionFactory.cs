using NUnit.Framework;

namespace MongoDB.Driver.Connections
{
    [TestFixture]
    public class TestConnectionFactory
    {
        [TearDown]
        public void TearDown()
        {
            ConnectionFactory.Shutdown();   
        }

        [Test]
        public void TestGetConnection()
        {
            var connection1 = ConnectionFactory.GetConnection(string.Empty);
            var connection2 = ConnectionFactory.GetConnection(string.Empty);
            Assert.IsNotNull(connection1);
            Assert.IsNotNull(connection2);
            Assert.AreEqual(1, ConnectionFactory.PoolCount);
        }

        [Test]
        public void TestCreatePoolForEachUniqeConnectionString()
        {
            ConnectionFactory.GetConnection(string.Empty);
            ConnectionFactory.GetConnection(string.Empty);
            ConnectionFactory.GetConnection("Username=test");
            ConnectionFactory.GetConnection("Username=test");
            ConnectionFactory.GetConnection("Server=localhost");
            Assert.AreEqual(3, ConnectionFactory.PoolCount);
        }
    }
}