
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
        
        protected override void WriteBody(Stream stream){
            BsonWriter writer = new BsonWriter(stream);     
            writer.Write(this.Message);
        }
    }
}
