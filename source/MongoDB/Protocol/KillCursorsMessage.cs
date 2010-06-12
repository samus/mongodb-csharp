using MongoDB.Bson;

namespace MongoDB.Protocol
{
    /// <summary>
    /// The OP_KILL_CURSORS message is used to close an active
    /// cursor in the database. This is necessary to ensure
    /// that database resources are reclaimed at the end of the query.
    /// </summary>
    /// <remarks>
    /// struct {
    /// MsgHeader header;                 // standard message header
    /// int32     ZERO;                   // 0 - reserved for future use
    /// int32     numberOfCursorIDs;      // number of cursorIDs in message
    /// int64[]   cursorIDs;                // array of cursorIDs to close
    /// }
    /// </remarks>
    internal class KillCursorsMessage : RequestMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KillCursorsMessage"/> class.
        /// </summary>
        public KillCursorsMessage()
            :base(new BsonWriterSettings()){
            Header = new MessageHeader(OpCode.KillCursors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KillCursorsMessage"/> class.
        /// </summary>
        /// <param name="cursorId">The cursor id.</param>
        public KillCursorsMessage(long cursorId) : this()
        {
            CursorIds = new[]{cursorId};
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KillCursorsMessage"/> class.
        /// </summary>
        /// <param name="cursorIDs">The cursor I ds.</param>
        public KillCursorsMessage(long[] cursorIDs) : this()
        {
            CursorIds = cursorIDs;
        }

        /// <summary>
        /// Gets or sets the cursor ids.
        /// </summary>
        /// <value>The cursor ids.</value>
        public long[] CursorIds { get; set; }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonType.Integer, 0);
            writer.WriteValue(BsonType.Integer, CursorIds.Length);

            foreach(var id in CursorIds)
                writer.WriteValue(BsonType.Long, id);
        }

        /// <summary>
        /// Calculates the size of the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        protected override int CalculateBodySize(BsonWriter writer){
            var size = 8; //first int32, number of cursors
            size += (CursorIds.Length*8);
            return size;
        }
    }
}