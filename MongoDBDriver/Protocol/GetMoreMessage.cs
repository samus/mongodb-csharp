using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// Description of GetMoreMessage.
    /// </summary>
    /// <remarks>
    ///      struct {
    ///          MsgHeader header;                 // standard message header
    ///          int32     ZERO;                   // 0 - reserved for future use
    ///          cstring   fullCollectionName;     // "dbname.collectionname"
    ///          int32     numberToReturn;         // number of documents to return
    ///          int64     cursorID;               // cursorID from the OP_REPLY
    ///      }
    /// </remarks>
    public class GetMoreMessage : RequestMessageBase
    {
        public long CursorID { get; set; }

        public string FullCollectionName { get; set; }

        public int NumberToReturn { get; set; }

        public GetMoreMessage(string fullCollectionName, long cursorID)
            :this(fullCollectionName, cursorID, 0){
        }
        
        public GetMoreMessage(string fullCollectionName, long cursorID, int numberToReturn){
            this.Header = new MessageHeader(OpCode.GetMore);
            this.FullCollectionName = fullCollectionName;
            this.CursorID = cursorID;
            this.NumberToReturn = numberToReturn;
        }
        
        protected override void WriteBody (BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.Write(this.FullCollectionName,false);
            writer.WriteValue(BsonDataType.Integer,this.NumberToReturn);
            writer.WriteValue(BsonDataType.Long,this.CursorID);
        }       
        
        protected override int CalculateBodySize(BsonWriter writer){
            int size = 4; //first int32
            size += writer.CalculateSize(this.FullCollectionName,false);
            size += 12; //number to return + cursorid
            return size;
        }
        
    }
}
