using System.IO;
using System.Net.Sockets;
using System.Text;
using NUnit.Framework;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestB
    {
        [Test]
        public void TestCase()
        {
            var client = new TcpClient();
            client.Connect("localhost", 27017);
            var buff = new BufferedStream(client.GetStream());
            var writer = new BinaryWriter(buff);

            var encoding = new UTF8Encoding();
            var msg = encoding.GetBytes("Hello MongoDB!");

            writer.Write(16 + msg.Length + 1);
            writer.Write(1);
            writer.Write(1);
            writer.Write(1000);
            writer.Write(msg);
            writer.Write((byte)0);

            writer.Flush();
            writer.Close();
            client.Close();
        }
    }
}