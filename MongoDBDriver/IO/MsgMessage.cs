
using System;
using System.IO;
using System.Text;

using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    
    
    public class MsgMessage : RequestMessage
    {
//      struct {
//          MsgHeader header;    // standard message header
//          cstring   message;   // message for the database
//      }
        private string message;
        public string Message {
            get {return message;}
            set {message = value;}
        }       
        
        public MsgMessage(){
            this.Header = new MessageHeader(OpCode.Msg);
        }

        protected override void WriteBody (BsonWriter2 writer){
            writer.WriteString(this.Message);            
        }
        
        protected override int CalculateBodySize(BsonWriter2 writer){
            return writer.CalculateSize(this.Message,false);
        }        
    }
}
