using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// The OP_KILL_CURSORS message is used to close an active 
    /// cursor in the database. This is necessary to ensure 
    /// that database resources are reclaimed at the end of the query.
    /// </summary>
    /// <remarks>
    /// struct {
    ///     MsgHeader header;                 // standard message header
    ///     int32     ZERO;                   // 0 - reserved for future use
    ///     int32     numberOfCursorIDs;      // number of cursorIDs in message
    ///     int64[]   cursorIDs;                // array of cursorIDs to close
    /// }
    /// </remarks>
    public class KillCursorsMessage : RequestMessageBase
    {
        public KillCursorsMessage()
            :base(new DocumentDescriptor()){
            Header = new MessageHeader(OpCode.KillCursors);
        }

        public KillCursorsMessage(long cursorId)
            :base(new DocumentDescriptor()){
            CursorIds = new[]{cursorId};
        }

        public KillCursorsMessage(long[] cursorIDs)
            : base(new DocumentDescriptor())
        {
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