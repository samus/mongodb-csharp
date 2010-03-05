using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///      struct {
    ///          MsgHeader header;    // standard message header
    ///          cstring   message;   // message for the database
    ///      }
    /// </remarks>
    public class MsgMessage : RequestMessageBase
    {
        public string Message { get; set; }

        public MsgMessage(){
            this.Header = new MessageHeader(OpCode.Msg);
        }
        
        protected override void WriteBody (BsonWriter writer){
            writer.WriteString(this.Message);            
        }
        
        protected override int CalculateBodySize(BsonWriter writer){
            return writer.CalculateSize(this.Message,false);
        }
    }
}