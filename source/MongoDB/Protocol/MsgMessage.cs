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
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgMessage"/> class.
        /// </summary>
        public MsgMessage()
            : base(new BsonDocumentDescriptor()){
            Header = new MessageHeader(OpCode.Msg);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteBody(BsonWriter writer){
            writer.Write(Message, false);
        }

        /// <summary>
        /// Calculates the size of the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        protected override int CalculateBodySize(BsonWriter writer){
            return writer.CalculateSize(Message, false);
        }
    }
}