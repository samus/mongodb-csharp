using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    ///   Description of KillCursorsMessage.
    /// </summary>
    /// <remarks>
    ///   struct {
    ///   MsgHeader header;                 // standard message header
    ///   int32     ZERO;                   // 0 - reserved for future use
    ///   int32     numberOfCursorIDs;      // number of cursorIDs in message
    ///   int64[]   cursorIDs;                // array of cursorIDs to close
    ///   }
    /// </remarks>
    public class KillCursorsMessage : RequestMessageBase
    {
        public KillCursorsMessage(){
            Header = new MessageHeader(OpCode.KillCursors);
        }

        public KillCursorsMessage(long cursorId)
            : this(){
            CursorIds = new[]{cursorId};
        }

        public KillCursorsMessage(long[] cursorIDs)
            : this(){
            CursorIds = cursorIDs;
        }

        public long[] CursorIds { get; set; }

        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer, 0);
            writer.WriteValue(BsonDataType.Integer, CursorIds.Length);

            foreach(var id in CursorIds)
                writer.WriteValue(BsonDataType.Long, id);
        }

        protected override int CalculateBodySize(BsonWriter writer){
            var size = 8; //first int32, number of cursors
            size += (CursorIds.Length*8);
            return size;
        }
    }
}