using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestConnectionStringBuilder
    {
        [Test]
        public void TestConnectionStringParsing(){
            var builder =
                new MongoConnectionStringBuilder(
                    "Username=testuser;Password=testpassword;Server=testserver:555;ConnectionLifetime=50;MaximumPoolSize=101;MinimumPoolSize=202;Pooled=false;Database=testdatabase");
            Assert.AreEqual("testuser", builder.Username);
            Assert.AreEqual("testpassword", builder.Password);
            Assert.AreEqual("testdatabase", builder.Database);
            Assert.AreEqual(101, builder.MaximumPoolSize);
            Assert.AreEqual(202, builder.MinimumPoolSize);
            Assert.AreEqual(TimeSpan.FromSeconds(50), builder.ConnectionLifetime);
            Assert.AreEqual(false, builder.Pooled);

            var servers = new List<MongoServerEndPoint>(builder.Servers);
            Assert.AreEqual(1, servers.Count);
            Assert.AreEqual("testserver", servers[0].Host);
            Assert.AreEqual(555, servers[0].Port);
        }

        [Test]
        public void TestConnectionStringParsingServerWithoutPort(){
            var builder = new MongoConnectionStringBuilder("Username=testuser;Password=testpassword;Server=testserver");
            Assert.AreEqual("testuser", builder.Username);
            Assert.AreEqual("testpassword", builder.Password);

            var servers = new List<MongoServerEndPoint>(builder.Servers);
            Assert.AreEqual(1, servers.Count);
            Assert.AreEqual("testserver", servers[0].Host);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, servers[0].Port);
        }

        [Test]
        public void TestCreateEmptyInstance(){
            new MongoConnectionStringBuilder();
        }

        [Test]
        public void TestDefaults(){
            var builder = new MongoConnectionStringBuilder();
            Assert.IsNull(builder.Username);
            Assert.IsNull(builder.Password);
            Assert.AreEqual(builder.MaximumPoolSize, MongoConnectionStringBuilder.DefaultMaximumPoolSize);
            Assert.AreEqual(builder.MinimumPoolSize, MongoConnectionStringBuilder.DefaultMinimumPoolSize);
            Assert.AreEqual(builder.ConnectionLifetime, MongoConnectionStringBuilder.DefaultConnectionLifeTime);
            Assert.AreEqual(builder.ConnectionTimeout, MongoConnectionStringBuilder.DefaultConnectionTimeout);
            Assert.AreEqual(builder.Database, MongoConnectionStringBuilder.DefaultDatabase);
            Assert.AreEqual(builder.Pooled, MongoConnectionStringBuilder.DefaultPooled);

            var servers = new List<MongoServerEndPoint>(builder.Servers);
            Assert.AreEqual(1, servers.Count);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, servers[0].Port);
            Assert.AreEqual(MongoServerEndPoint.DefaultHost, servers[0].Host);
        }

        [Test]
        public void TestSimpleUriString(){
            var builder = new MongoConnectionStringBuilder("mongodb://server");
            Assert.AreEqual(1, builder.Servers.Length);
            Assert.AreEqual(MongoConnectionStringBuilder.DefaultDatabase, builder.Database);
            Assert.AreEqual("server", builder.Servers[0].Host);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, builder.Servers[0].Port);
        }

        [Test]
        public void TestSimpleUriStringWithDatabase(){
            var builder = new MongoConnectionStringBuilder("mongodb://server/database");
            Assert.AreEqual("database", builder.Database);
            Assert.AreEqual(1, builder.Servers.Length);
            Assert.AreEqual("server", builder.Servers[0].Host);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, builder.Servers[0].Port);
        }

        [Test]
        public void TestToStringOutput(){
            var builder = new MongoConnectionStringBuilder
            {
                Password = "testpassword",
                Username = "testusername",
                ConnectionLifetime = TimeSpan.FromSeconds(50),
                MaximumPoolSize = 101,
                MinimumPoolSize = 202,
                ConnectionTimeout = TimeSpan.FromSeconds(60)
            };
            builder.AddServer("testserver1", 555);
            builder.AddServer("testserver2");

            Assert.AreEqual(
                "Username=testusername;Password=testpassword;Server=testserver1:555,testserver2;MaximumPoolSize=101;MinimumPoolSize=202;ConnectionTimeout=60;ConnectionLifetime=50",
                builder.ToString());
        }

        [Test]
        public void TestToStringOutputWithDefaultServerPort(){
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("testserver");
            Assert.AreEqual("Server=testserver", builder.ToString());
        }

        [Test]
        public void TestToStringOutputWithoutUsernameAndPassword(){
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("testserver", 555);

            Assert.AreEqual("Server=testserver:555", builder.ToString());
        }

        [Test]
        public void TestUriStringWithUsenameAndPasswort(){
            var builder = new MongoConnectionStringBuilder("mongodb://username:password@server");
            Assert.AreEqual("username", builder.Username);
            Assert.AreEqual("password", builder.Password);
            Assert.AreEqual(MongoConnectionStringBuilder.DefaultDatabase, builder.Database);
            Assert.AreEqual(1, builder.Servers.Length);
            Assert.AreEqual("server", builder.Servers[0].Host);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, builder.Servers[0].Port);
        }

        [Test]
        public void TestUriWithMultipleServers(){
            var builder = new MongoConnectionStringBuilder("mongodb://server1,server2:1234,server3/database");
            Assert.AreEqual("database", builder.Database);
            Assert.AreEqual(3, builder.Servers.Length);
            Assert.AreEqual("server1", builder.Servers[0].Host);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, builder.Servers[0].Port);
            Assert.AreEqual("server2", builder.Servers[1].Host);
            Assert.AreEqual(1234, builder.Servers[1].Port);
            Assert.AreEqual("server3", builder.Servers[2].Host);
            Assert.AreEqual(MongoServerEndPoint.DefaultPort, builder.Servers[2].Port);
        }

        [Test]
        public void TestUriWithPropertys(){
            var builder = new MongoConnectionStringBuilder("mongodb://server1/database?pooled=false&connectionlifetime=10");
            Assert.AreEqual(false, builder.Pooled);
            Assert.AreEqual(10.0, builder.ConnectionLifetime.TotalSeconds);
        }
    }
}