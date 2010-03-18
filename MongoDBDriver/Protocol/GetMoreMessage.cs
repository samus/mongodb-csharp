using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    ///   Description of GetMoreMessage.
    /// </summary>
    /// <remarks>
    ///   struct {
    ///   MsgHeader header;                 // standard message header
    ///   int32     ZERO;                   // 0 - reserved for future use
    ///   cstring   fullCollectionName;     // "dbname.collectionname"
    ///   int32     numberToReturn;         // number of documents to return
    ///   int64     cursorID;               // cursorID from the OP_REPLY
    ///   }
    /// </remarks>
    public class GetMoreMessage : RequestMessageBase
    {
        public GetMoreMessage(string fullCollectionName, long cursorID)
            : this(fullCollectionName, cursorID, 0){
        }

        public GetMoreMessage(string fullCollectionName, long cursorID, int numberToReturn){
            Header = new MessageHeader(OpCode.GetMore);
            FullCollectionName = fullCollectionName;
            CursorId = cursorID;
            NumberToReturn = numberToReturn;
        }

        public long CursorId { get; set; }

        public string FullCollectionName { get; set; }

        public int NumberToReturn { get; set; }

        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer, 0);
            writer.Write(FullCollectionName, false);
            writer.WriteValue(BsonDataType.Integer, NumberToReturn);
            writer.WriteValue(BsonDataType.Long, CursorId);
        }

        protected override int CalculateBodySize(BsonWriter writer){
            var size = 4; //first int32
            size += writer.CalculateSize(FullCollectionName, false);
            size += 12; //number to return + cursorid
            return size;
        }
    }
}