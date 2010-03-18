/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;
using System.Diagnostics.CodeAnalysis;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    ///   In general, each Message consists of a standard message header followed by request-specific data.
    /// </summary>
    public class MessageHeader
    {
        // total size of the message, including the 4 bytes of length 
        public MessageHeader(OpCode opCode){
            OpCode = opCode;
            MessageLength = 16; //The starting size of any message.
        }

        public int MessageLength { get; set; }

        // client or database-generated identifier for this message
        public int RequestId { get; set; }

        // requestID from the original request (used in reponses from db)     
        public int ResponseTo { get; set; }

        // request type - see table below    
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public OpCode OpCode { get; set; }

        public override String ToString(){
            return "length:" + MessageLength + " requestId:" + RequestId + " responseTo:" + ResponseTo + " opCode:" + OpCode;
        }
    }
}