using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// Deprecated. OP_MSG sends a diagnostic message to the database.  
    /// The database sends back a fixed resonse.
    /// </summary>
    /// <remarks>
    /// struct {
    ///     MsgHeader header;    // standard message header
    ///     cstring   message;   // message for the database
    /// }
    /// </remarks>
    public class MsgMessage : RequestMessageBase
    {
        public MsgMessage()
            : base(new DocumentDescriptor()){
            Header = new MessageHeader(OpCode.Msg);
        }

        public string Message { get; set; }

        protected override void WriteBody(BsonWriter writer){
            writer.Write(Message, false);
        }

        protected override int CalculateBodySize(BsonWriter writer){
            return writer.CalculateSize(Message, false);
        }
    }
}