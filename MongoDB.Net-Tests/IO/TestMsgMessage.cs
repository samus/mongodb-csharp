
using System;
using System.IO;

using NUnit.Framework;

namespace MongoDB.Driver.IO
{
    [TestFixture()]
    public class TestMsgMessage
    {
        
        [Test()]
        public void TestAllBytesWritten(){
            MsgMessage msg = new MsgMessage();
            msg.Header.RequestId = 1;
            msg.Header.ResponseTo = 11;
            Assert.AreEqual(16,msg.Header.MessageLength);
            
            msg.Message = "A";
            MemoryStream buffer = new MemoryStream(18);
            msg.Write(buffer);
            
            Byte[] output = buffer.ToArray();
            String hexdump = BitConverter.ToString(output);
            //Console.WriteLine("Dump: " + hexdump);

            Assert.IsTrue(output.Length > 0);
            Assert.AreEqual(output.Length, msg.Header.MessageLength);
            Assert.AreEqual("12-00-00-00-01-00-00-00-0B-00-00-00-E8-03-00-00-41-00", hexdump);
        }
    }
}
