using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestConnectionStringBuilder
    {
        [Test]
        public void TestCreateEmptyInstance (){
            new MongoConnectionStringBuilder ();
        }

        [Test]
        public void TestDefaults (){
            var builder = new MongoConnectionStringBuilder ();
            Assert.IsNull (builder.Username);
            Assert.IsNull (builder.Password);
            Assert.AreEqual (builder.MaximumPoolSize, MongoConnectionStringBuilder.DefaultMaximumPoolSize);
            Assert.AreEqual (builder.MinimumPoolSize, MongoConnectionStringBuilder.DefaultMinimumPoolSize);
            Assert.AreEqual (builder.ConnectionLifetime, MongoConnectionStringBuilder.DefaultConnectionLifeTime);
            
            var servers = new List<MongoServerEndPoint> (builder.Servers);
            Assert.AreEqual (1, servers.Count);
            Assert.AreEqual (MongoServerEndPoint.DefaultPort, servers[0].Port);
            Assert.AreEqual (MongoServerEndPoint.DefaultHost, servers[0].Host);
        }

        [Test]
        public void TestConnectionStringParsing (){
            var builder = new MongoConnectionStringBuilder ("Username=testuser;Passwort=testpassword;Server=testserver:555;ConnectionLifetime=50;MaximumPoolSize=101;MinimumPoolSize=202");
            Assert.AreEqual ("testuser", builder.Username);
            Assert.AreEqual ("testpassword", builder.Password);
            Assert.AreEqual (101, builder.MaximumPoolSize);
            Assert.AreEqual (202, builder.MinimumPoolSize);
            Assert.AreEqual (TimeSpan.FromSeconds (50), builder.ConnectionLifetime);
            
            var servers = new List<MongoServerEndPoint> (builder.Servers);
            Assert.AreEqual (1, servers.Count);
            Assert.AreEqual ("testserver", servers[0].Host);
            Assert.AreEqual (555, servers[0].Port);
        }

        [Test]
        public void TestConnectionStringParsingServerWithoutPort (){
            var builder = new MongoConnectionStringBuilder ("Username=testuser;Passwort=testpassword;Server=testserver");
            Assert.AreEqual ("testuser", builder.Username);
            Assert.AreEqual ("testpassword", builder.Password);
            
            var servers = new List<MongoServerEndPoint> (builder.Servers);
            Assert.AreEqual (1, servers.Count);
            Assert.AreEqual ("testserver", servers[0].Host);
            Assert.AreEqual (MongoServerEndPoint.DefaultPort, servers[0].Port);
        }

        [Test]
        public void TestToStringOutput (){
            var builder = new MongoConnectionStringBuilder { Password = "testpassword", Username = "testusername", ConnectionLifetime = TimeSpan.FromSeconds (50), MaximumPoolSize = 101, MinimumPoolSize = 202 };
            builder.AddServer ("testserver1", 555);
            builder.AddServer ("testserver2");
            
            Assert.AreEqual ("Username=testusername;Passwort=testpassword;Server=testserver1:555,testserver2;MaximumPoolSize=101;MinimumPoolSize=202;ConnectionLifetime=50", builder.ToString ());
        }

        [Test]
        public void TestToStringOutputWithoutUsernameAndPassword (){
            var builder = new MongoConnectionStringBuilder ();
            builder.AddServer ("testserver", 555);
            
            Assert.AreEqual ("Server=testserver:555", builder.ToString ());
        }

        [Test]
        public void TestToStringOutputWithDefaultServerPort (){
            var builder = new MongoConnectionStringBuilder ();
            builder.AddServer ("testserver");
            Assert.AreEqual ("Server=testserver", builder.ToString ());
        }

    }
}
