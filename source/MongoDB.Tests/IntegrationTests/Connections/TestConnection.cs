using System.IO;
using System.Text;
using MongoDB.Bson;
using MongoDB.Connections;
using MongoDB.Protocol;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.Connections
{
    [TestFixture]
    public class TestConnection
    {
        private void WriteBadMessage(Connection conn)
        {
            //Write a bad message to the socket to force mongo to shut down our connection.
            var writer = new BinaryWriter(conn.GetStream());
            var encoding = new UTF8Encoding();
            var msg = encoding.GetBytes("Goodbye MongoDB!");
            writer.Write(16 + msg.Length + 1);
            writer.Write(1);
            writer.Write(1);
            writer.Write(1001);
            writer.Write(msg);
            writer.Write((byte)0);
        }

        private QueryMessage GenerateQueryMessage()
        {
            var qdoc = new Document {{"listDatabases", 1.0}};
            //QueryMessage qmsg = new QueryMessage(qdoc,"system.namespaces");
            return new QueryMessage(new BsonWriterSettings(), qdoc, "admin.$cmd")
            {
                NumberToReturn = -1
            };
        }

        [Test]
        public void TestReconnectOnce()
        {
            var conn = ConnectionFactoryFactory.GetConnection(string.Empty);
            conn.Open();

            WriteBadMessage(conn);
            try
            {
                var qmsg = GenerateQueryMessage();
                conn.SendTwoWayMessage(qmsg,string.Empty);
            }
            catch(IOException)
            {
                //Should be able to resend.
                Assert.IsTrue(conn.IsConnected);
                var qmsg = GenerateQueryMessage();
                var rmsg = conn.SendTwoWayMessage(qmsg, string.Empty);
                Assert.IsNotNull(rmsg);
            }
        }

        [Test]
        public void TestSendQueryMessage()
        {
            //Connection conn = new Connection("10.141.153.2");
            var conn = ConnectionFactoryFactory.GetConnection(string.Empty);
            conn.Open();

            var qmsg = GenerateQueryMessage();
            conn.SendTwoWayMessage(qmsg, string.Empty);

            conn.Close();
        }
    }
}