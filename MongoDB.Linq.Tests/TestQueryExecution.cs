using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using NUnit.Framework;

namespace MongoDB.Linq.Tests {
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestQueryExecution {
        private Mongo mongo;

        [TestFixtureSetUp]
        public void GlobalSetup() {
            Debug.WriteLine("initiallizing connection");
            mongo = AppSettingsFactory.CreateMongo();
            mongo.Connect();
        }

        [TestFixtureTearDown]
        public void GlobalTeardown() {
            mongo.Disconnect();
        }

        [Test]
        public void Can_build_simple_query() {
            var c = mongo["foo"]["bar"];
            var q = from d in c.AsQueryable() where (string)d["name"] == "bob" select d;
            var l = q.ToList();
        }
    }
    // ReSharper restore InconsistentNaming
}
