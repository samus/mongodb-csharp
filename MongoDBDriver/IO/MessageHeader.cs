/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;

namespace MongoDB.Driver.IO
{    /// <summary>
    /// In general, each Message consists of a standard message header followed by request-specific data.
    /// </summary>
    public class MessageHeader
    {
        private Int32 messageLength;  // total size of the message, including the 4 bytes of length 
        public int MessageLength {
            get { return messageLength; }
            set { messageLength = value; }
        }
        
        private Int32 requestId;      // client or database-generated identifier for this message
        public int RequestId {
            get { return requestId; }
            set { requestId = value; }
        }
        
        private Int32 responseTo;     // requestID from the original request (used in reponses from db)     
        public int ResponseTo {
            get { return responseTo; }
            set { responseTo = value; }
        }
        
        private OpCode opCode;         // request type - see table below    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public OpCode OpCode {
            get { return opCode; }
            set { opCode = value; }
        }
        
        public MessageHeader(OpCode opCode)
        {
            this.OpCode = opCode;
            this.MessageLength = 16; //The starting size of any message.
        }
        
        public override String ToString(){
            return "length:" + this.messageLength + " requestId:" + this.requestId + " responseTo:" + this.responseTo + " opCode:" + this.opCode;
        }
        
    }
}
