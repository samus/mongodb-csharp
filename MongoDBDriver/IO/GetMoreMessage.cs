/*
 * User: scorder
 */
using System;
using System.IO;
using System.Text;


using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    /// <summary>
    /// Description of GetMoreMessage.
    /// </summary>
    public class GetMoreMessage : RequestMessage
    {
//      struct {
//          MsgHeader header;                 // standard message header
//          int32     ZERO;                   // 0 - reserved for future use
//          cstring   fullCollectionName;     // "dbname.collectionname"
//          int32     numberToReturn;         // number of documents to return
//          int64     cursorID;               // cursorID from the OP_REPLY
//      }
#region "Properties"
        private long cursorID;  
        public long CursorID {
            get { return cursorID; }
            set { cursorID = value; }
        }
        
        private string fullCollectionName;      
        public string FullCollectionName {
            get { return fullCollectionName; }
            set { fullCollectionName = value; }
        }
        
        private Int32 numberToReturn;
        public int NumberToReturn {
            get { return numberToReturn; }
            set { numberToReturn = value; }
        }
#endregion
        public GetMoreMessage(string fullCollectionName, long cursorID)
            :this(fullCollectionName, cursorID, 0){
        }
        
        public GetMoreMessage(string fullCollectionName, long cursorID, int numberToReturn){
            this.Header = new MessageHeader(OpCode.GetMore);
            this.FullCollectionName = fullCollectionName;
            this.CursorID = cursorID;
            this.NumberToReturn = numberToReturn;
        }
        
        protected override void WriteBody (BsonWriter2 writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.WriteString(this.FullCollectionName);
            writer.WriteValue(BsonDataType.Integer,this.NumberToReturn);
            writer.WriteValue(BsonDataType.Long,this.CursorID);
        }       
        
        protected override int CalculateBodySize(BsonWriter2 writer){
            int size = 4; //first int32
            size += writer.CalculateSize(this.FullCollectionName,false);
            size += 12; //number to return + cursorid
            return size;
        }
        
    }
}
