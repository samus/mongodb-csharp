using System.IO;
using MongoDB.Driver.Bson;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    ///   Description of Message.
    /// </summary>
    public abstract class RequestMessageBase : MessageBase, IRequestMessage
    {
        public void Write (Stream stream){
            var header = Header;
            var bstream = new BufferedStream(stream);
            var writer = new BinaryWriter(bstream);
            var bwriter = new BsonWriter(bstream,new ReflectionDescriptor());
            
            Header.MessageLength += this.CalculateBodySize(bwriter);
            if(Header.MessageLength > MessageBase.MaximumMessageSize){
                throw new MongoException("Maximum message length exceeded");
            }
            writer.Write(header.MessageLength);
            writer.Write(header.RequestId);
            writer.Write(header.ResponseTo);
            writer.Write((int)header.OpCode);
            writer.Flush();
            WriteBody(bwriter);
            bwriter.Flush();
        }

        protected abstract void WriteBody(BsonWriter writer);

        protected abstract int CalculateBodySize(BsonWriter writer);
    }
}