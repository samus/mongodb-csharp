/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;

namespace MongoDB.Driver.Protocol
{    /// <summary>
    /// In general, each Message consists of a standard message header followed by request-specific data.
    /// </summary>
    public class MessageHeader
    {
        // total size of the message, including the 4 bytes of length 
        public int MessageLength { get; set; }

        // client or database-generated identifier for this message
        public int RequestId { get; set; }

        // requestID from the original request (used in reponses from db)     
        public int ResponseTo { get; set; }

        // request type - see table below    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public OpCode OpCode { get; set; }
        
        public MessageHeader(OpCode opCode)
        {
            this.OpCode = opCode;
            this.MessageLength = 16; //The starting size of any message.
        }
        
        public override String ToString(){
            return "length:" + this.MessageLength + " requestId:" + this.RequestId + " responseTo:" + this.ResponseTo + " opCode:" + this.OpCode;
        }
        
    }
}
