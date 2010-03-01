using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture()]
    public class TestConnectionStringBuilder
    {
        [Test]
        public void TestCreateEmptyInstance()
        {
            new MongoConnectionStringBuilder();
        }

        [Test]
        public void TestDefaults()
        {
            var builder = new MongoConnectionStringBuilder();
            Assert.IsNull(builder.Username);
            Assert.IsNull(builder.Password);

            var servers = new List<MongoServerEndPoint>(builder.Servers);
            Assert.AreEqual(1,servers.Count);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, servers[0].Port);
            Assert.AreEqual(MongoServerEndPoint.DefaultHost, servers[0].Host);
        }

        [Test]
        public void TestConnectionStringParsing()
        {
            var builder = new MongoConnectionStringBuilder("Username=testuser;Passwort=testpassword;Server=testserver:555");
            Assert.AreEqual("testuser", builder.Username);
            Assert.AreEqual("testpassword", builder.Password);

            var servers = new List<MongoServerEndPoint>(builder.Servers);
            Assert.AreEqual(1,servers.Count);
            Assert.AreEqual("testserver", servers[0].Host);
            Assert.AreEqual(555, servers[0].Port);
        }

        [Test]
        public void TestConnectionStringParsingServerWithoutPort()
        {
            var builder = new MongoConnectionStringBuilder("Username=testuser;Passwort=testpassword;Server=testserver");
            Assert.AreEqual("testuser", builder.Username);
            Assert.AreEqual("testpassword", builder.Password);

            var servers = new List<MongoServerEndPoint>(builder.Servers);
            Assert.AreEqual(1, servers.Count);
            Assert.AreEqual("testserver", servers[0].Host);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, servers[0].Port);
        }

        [Test]
        public void TestToStringOutput()
        {
            var builder = new MongoConnectionStringBuilder
            {
                Password = "testpassword", 
                Username = "testusername"
            };
            builder.AddServer("testserver",555);

            Assert.AreEqual("Username=testusername;Passwort=testpassword;Server=testserver:555", builder.ToString());
        }

        [Test]
        public void TestToStringOutputWithoutUsernameAndPasswort()
        {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("testserver", 555);

            Assert.AreEqual("Server=testserver:555", builder.ToString());
        }

        [Test]
        public void TestToStringOutputWithDefaultServerPort()
        {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("testserver");
            Assert.AreEqual("Server=testserver", builder.ToString());
        }
    }
}