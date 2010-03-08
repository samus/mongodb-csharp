using System;
using System.Threading;
using NUnit.Framework;

namespace MongoDB.Driver.Connections
{
    [TestFixture]
    public class TestPooledConnectionFactory
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            ConnectionFactory.Shutdown();
        }

        [Test]
        public void TestDefaultPoolSizeIsEmpty()
        {
            using(var pool = new PooledConnectionFactory(string.Empty))
                Assert.AreEqual(0, pool.PoolSize);
        }

        [Test]
        public void TestMinimalPoolSizeIsEnsuredAtStartup()
        {
            var builder = new MongoConnectionStringBuilder {MinimumPoolSize = 3};
            using(var pool = new PooledConnectionFactory(builder.ToString()))
                Assert.AreEqual(3, pool.PoolSize);
        }

        [Test]
        public void TestMinimalPoolSizeIsEnsuredAtRuntime()
        {
            var builder = new MongoConnectionStringBuilder { MinimumPoolSize = 3, ConnectionLifetime = TimeSpan.FromMilliseconds(200)};
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                Assert.AreEqual(3, pool.PoolSize);

                Thread.Sleep(500); // ensure connection lifetime reached

                pool.Cleanup();

                Assert.AreEqual(3, pool.PoolSize);
            }
        }

        [Test]
        public void TestMaximumPoolSizeIsEnsured()
        {
            var builder = new MongoConnectionStringBuilder { MaximumPoolSize = 2 };
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                var connection1 = pool.Open();
                Assert.IsNotNull(connection1);
                Assert.IsNotNull(pool.Open());

                RawConnection connection3 = null;
                var thread = new Thread(o =>
                {
                    connection3 = pool.Open();
                });
                thread.Start();
                
                Thread.Sleep(300);
                
                Assert.IsNull(connection3);

                pool.Close(connection1);
                
                Thread.Sleep(300);

                Assert.IsNotNull(connection3);

                thread.Abort();
            }
        }

        [Test]
        [ExpectedException(typeof(MongoException))]
        public void TestExceptionIfMaximumPoolSizeAndConnectionTimeoutAreReached()
        {
            var builder = new MongoConnectionStringBuilder { MaximumPoolSize = 1, ConnectionTimeout = TimeSpan.FromMilliseconds(500)};
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                pool.Open();
                pool.Open();
            }
        }

        [Test]
        public void TestServerCirculationWorks()
        {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("localhost");
            builder.AddServer("localhost", 27018);
            builder.AddServer("localhost", 27019);
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                var connection1 = pool.Open();
                var connection2 = pool.Open();
                var connection3 = pool.Open();
                var connection4 = pool.Open();
                var connection5 = pool.Open();
                Assert.AreEqual(27017, connection1.EndPoint.Port);
                Assert.AreEqual(27018, connection2.EndPoint.Port);
                Assert.AreEqual(27019, connection3.EndPoint.Port);
                Assert.AreEqual(27017, connection4.EndPoint.Port);
                Assert.AreEqual(27018, connection5.EndPoint.Port);
            }
        }

        [Test]
        public void TestBorrowOneConnection()
        {
            var builder = new MongoConnectionStringBuilder();
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                var connection = pool.Open();
                Assert.IsNotNull(connection);
                Assert.AreEqual(1, pool.PoolSize);
            }
        }

        [Test]
        public void TestClose()
        {
            using(var pool = new PooledConnectionFactory(string.Empty))
            {
                var connection1 = pool.Open();
                pool.Close(connection1);
                var connection2 = pool.Open();
                Assert.AreEqual(connection1,connection2);
                Assert.AreEqual(1, pool.PoolSize);
            }
        }

        [Test]
        public void TestInvalidConnectionsArentReturndToPool()
        {
            using(var pool = new PooledConnectionFactory(string.Empty))
            {
                var connection = pool.Open();
                connection.MarkAsInvalid();
                pool.Close(connection);
                Assert.AreEqual(0,pool.PoolSize);
            }
        }

        [Test]
        public void TestDisconnectedConnectionsArentReturndToPool()
        {
            using(var pool = new PooledConnectionFactory(string.Empty))
            {
                var connection = pool.Open();
                connection.Dispose();
                pool.Close(connection);
                Assert.AreEqual(0, pool.PoolSize);
            }
        }

        [Test]
        public void TestIfConnectionLifetimeIsReachedItDosenotReturndToPool()
        {
            var builder = new MongoConnectionStringBuilder
            {
                ConnectionLifetime = TimeSpan.FromMilliseconds(100)
            };
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                var connection = pool.Open();
                Thread.Sleep(200); // wait for lifetime reached
                pool.Close(connection);
                Assert.AreEqual(0, pool.PoolSize);
            }
        }

        [Test]
        public void TestCleanup()
        {
            var builder = new MongoConnectionStringBuilder
            {
                MinimumPoolSize = 0,
                ConnectionLifetime = TimeSpan.FromMilliseconds(100)
            };
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                var connection1 = pool.Open();
                var connection2 = pool.Open();
                var connection3 = pool.Open();
                pool.Close(connection1);
                pool.Close(connection2);
                pool.Close(connection3);

                Thread.Sleep(300); // ensure lifetime reached

                pool.Cleanup();

                Assert.AreEqual(0,pool.PoolSize);
            }
        }
    }
}