using System.Configuration;
using NUnit.Framework;

namespace MongoDB.Driver.IntegrationTests
{
    [TestFixture]
    public class TestMongo
    {
        private readonly string _connectionString = ConfigurationManager.AppSettings["tests"];

        [Test]
        public void TestDefaults(){
            using(var m = new Mongo())
            {
                //Connection string not needed since connect not called and it would screw up the test.
                Assert.AreEqual(string.Empty, m.ConnectionString);
            }
        }

        [Test]
        public void TestExplicitConnection(){
            using(var m = new Mongo(_connectionString))
                Assert.IsTrue(m.TryConnect());
        }

        [Test]
        [ExpectedException(typeof(MongoConnectionException))]
        public void TestConnectThrowIfConnectionFailed(){
            using(var m = new Mongo("Server=notexists"))
                m.Connect();
        }

        [Test]
        public void TestTryConnectDoseNotThrowIfConnectionFailed(){
            using(var m = new Mongo("Server=notexists"))
                Assert.IsFalse(m.TryConnect());
        }

        [Test]
        [ExpectedException(typeof(MongoConnectionException))]
        public void TestThatConnectMustBeCalled(){
            using(var m = new Mongo(_connectionString))
            {
                var db = m["admin"];
                db["$cmd"].FindOne(new Document().Add("listDatabases", 1.0));
            }
        }

        public void TestDoseNotThrowIfDisposeIsCalledTwice(){
            using(var m = new Mongo(_connectionString))
            {
                m.Dispose();
            }
        }
    }
}