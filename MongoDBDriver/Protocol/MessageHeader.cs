using System;
using System.Diagnostics.CodeAnalysis;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// In general, each Message consists of a standard message header followed by request-specific data.
    /// </summary>
    /// <remarks>
    /// struct {
    ///      int32   messageLength;  // total size of the message, including the 4 bytes of length
    ///      int32   requestID;      // client or database-generated identifier for this message
    ///      int32   responseTo;     // requestID from the original request (used in reponses from db)
    ///      int32   opCode;         // request type - see table below
    /// }
    /// </remarks>
    public class MessageHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader"/> class.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        public MessageHeader(OpCode opCode){
            OpCode = opCode;
            MessageLength = 16; //The starting size of any message.
        }

        /// <summary>
        /// Total size of the message, including the 4 bytes of length.
        /// </summary>
        /// <value>The length of the message.</value>
        public int MessageLength { get; set; }

        /// <summary>
        /// Client or database-generated identifier for this message.
        /// </summary>
        /// <value>The request id.</value>
        public int RequestId { get; set; }

        /// <summary>
        /// RequestID from the original request (used in reponses from db).
        /// </summary>
        /// <value>The response to.</value>
        public int ResponseTo { get; set; }

        /// <summary>
        /// Request type
        /// </summary>
        /// <value>The op code.</value>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        public OpCode OpCode { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override String ToString(){
            return "length:" + MessageLength + " requestId:" + RequestId + " responseTo:" + ResponseTo + " opCode:" + OpCode;
        }
    }
}