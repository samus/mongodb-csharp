
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;


using NUnit.Framework;

namespace MongoDB.Driver
{
    
    
    [TestFixture()]
    public class TestB
    {
        
        [Test()]
        public void TestCase(){
            TcpClient client = new TcpClient();
            client.Connect("localhost", 27017);
            BufferedStream buff = new BufferedStream(client.GetStream());
            BinaryWriter writer = new BinaryWriter(buff);

            System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();          
            Byte[] msg = encoding.GetBytes("Hello MongoDB!");
            
                
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
