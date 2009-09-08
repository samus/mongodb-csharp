/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;

namespace MongoDB.Driver.IO
{
    public enum OpCode{
        Reply = 1, //Reply to a client request. responseTo is set
        Msg = 1000, //generic msg command followed by a string
        Update = 2001, //update document
        Insert = 2002, //insert new document
        GetByOID = 2003, //is this used?
        Query = 2004, //query a collection
        GetMore = 2005, //Get more data from a query. See Cursors
        Delete = 2006, //Delete documents
        KillCursors = 2007 //Tell database client is done with a cursor         
    }
    
    /// <summary>
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
