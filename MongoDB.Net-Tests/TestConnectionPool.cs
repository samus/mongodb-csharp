using System;
using System.Threading;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestConnectionPool
    {
        [Test]
        public void TestDefaultPoolSizeIsEmpty()
        {
            using(var pool = new ConnectionPool(string.Empty))
                Assert.AreEqual(0, pool.PoolSize);
        }

        [Test]
        public void TestMinimalPoolSizeIsEnsured()
        {
            var builder = new MongoConnectionStringBuilder {MinimumPoolSize = 3};
            using(var pool = new ConnectionPool(builder.ToString()))
                Assert.AreEqual(3, pool.PoolSize);
        }

        [Test]
        public void TestMaximumPoolSizeIsEnsured()
        {
            var builder = new MongoConnectionStringBuilder { MaximumPoolSize = 2 };
            using(var pool = new ConnectionPool(builder.ToString()))
            {
                var connection1 = pool.BorrowConnection();
                Assert.IsNotNull(connection1);
                Assert.IsNotNull(pool.BorrowConnection());

                RawConnection connection3 = null;
                var thread = new Thread(o =>
                {
                    connection3 = pool.BorrowConnection();
                });
                thread.Start();
                
                Thread.Sleep(300);
                
                Assert.IsNull(connection3);

                pool.ReturnConnection(connection1);
                
                Thread.Sleep(300);

                Assert.IsNotNull(connection3);

                thread.Abort();
            }
        }

        [Test]
        public void TestServerCirculationWorks()
        {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("localhost");
            builder.AddServer("localhost", 27018);
            builder.AddServer("localhost", 27019);
            using(var pool = new ConnectionPool(builder.ToString()))
            {
                var connection1 = pool.BorrowConnection();
                var connection2 = pool.BorrowConnection();
                var connection3 = pool.BorrowConnection();
                var connection4 = pool.BorrowConnection();
                var connection5 = pool.BorrowConnection();
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
            using(var pool = new ConnectionPool(builder.ToString()))
            {
                var connection = pool.BorrowConnection();
                Assert.IsNotNull(connection);
                Assert.AreEqual(1, pool.PoolSize);
            }
        }

        [Test]
        public void TestReturnConnection()
        {
            using(var pool = new ConnectionPool(string.Empty))
            {
                var connection1 = pool.BorrowConnection();
                pool.ReturnConnection(connection1);
                var connection2 = pool.BorrowConnection();
                Assert.AreEqual(connection1,connection2);
                Assert.AreEqual(1, pool.PoolSize);
            }
        }

        [Test]
        public void TestInvalidConnectionsArentReturndToPool()
        {
            using(var pool = new ConnectionPool(string.Empty))
            {
                var connection = pool.BorrowConnection();
                connection.MarkAsInvalid();
                pool.ReturnConnection(connection);
                Assert.AreEqual(0,pool.PoolSize);
            }
        }

        [Test]
        public void TestIfConnectionLifetimeIsReachedItDosenotReturndToPool()
        {
            var builder = new MongoConnectionStringBuilder
            {
                ConnectionLifetime = TimeSpan.FromMilliseconds(100)
            };
            using(var pool = new ConnectionPool(builder.ToString()))
            {
                var connection = pool.BorrowConnection();
                Thread.Sleep(200); // wait for lifetime reached
                pool.ReturnConnection(connection);
                Assert.AreEqual(0, pool.PoolSize);
            }
        }

        [Test]
        public void TestCleanup()
        {
            var builder = new MongoConnectionStringBuilder
            {
                ConnectionLifetime = TimeSpan.FromMilliseconds(100)
            };
            using(var pool = new ConnectionPool(builder.ToString()))
            {
                var connection1 = pool.BorrowConnection();
                var connection2 = pool.BorrowConnection();
                var connection3 = pool.BorrowConnection();
                pool.ReturnConnection(connection1);
                pool.ReturnConnection(connection2);
                pool.ReturnConnection(connection3);

                Thread.Sleep(300); // ensure lifetime reached

                pool.Cleanup();

                Assert.AreEqual(0,pool.PoolSize);
            }
        }
    }
}