/*
 * User: scorder
 * Date: 7/21/2009
 */
using System;
using System.Diagnostics;
using System.IO;


namespace MongoDB.Driver.IO
{
    /// <summary>
    /// Description of Message.
    /// </summary>
    public abstract class RequestMessage : Message
    {
        
        public RequestMessage()
        {}
        
        public void Write (Stream stream){
            MessageHeader header = this.Header;
            BufferedStream bstream = new BufferedStream(stream);
            BinaryWriter writer = new BinaryWriter(bstream);
            
            MemoryStream bodyBuffer = new MemoryStream();
            this.WriteBody(bodyBuffer);
            //Byte[] body = bodyBuffer.ToArray();
            //header.MessageLength += body.Length;
            header.MessageLength += (int)bodyBuffer.Position;
            
            writer.Write(header.MessageLength);
            writer.Write(header.RequestId);
            writer.Write(header.ResponseTo);
            writer.Write((int)header.OpCode);
            bodyBuffer.WriteTo(bstream);
            //writer.Write(body);
            writer.Flush();
            Debug.WriteLine(header, "Request Message");
        }
        
        protected abstract void WriteBody(Stream stream);

    }
}
