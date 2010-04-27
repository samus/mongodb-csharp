using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// The OP_GETMORE message is used to query the database for documents in a collection.
    /// </summary>
    /// <remarks>
    /// struct {
    ///     MsgHeader header;                 // standard message header
    ///     int32     ZERO;                   // 0 - reserved for future use
    ///     cstring   fullCollectionName;     // "dbname.collectionname"
    ///     int32     numberToReturn;         // number of documents to return
    ///     int64     cursorID;               // cursorID from the OP_REPLY
    /// }
    /// </remarks>
    public class GetMoreMessage : RequestMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetMoreMessage"/> class.
        /// </summary>
        /// <param name="fullCollectionName">Full name of the collection.</param>
        /// <param name="cursorId">The cursor id.</param>
        public GetMoreMessage(string fullCollectionName, long cursorId)
            : this(fullCollectionName, cursorId, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMoreMessage"/> class.
        /// </summary>
        /// <param name="fullCollectionName">Full name of the collection.</param>
        /// <param name="cursorId">The cursor id.</param>
        /// <param name="numberToReturn">The number to return.</param>
        public GetMoreMessage(string fullCollectionName, long cursorId, int numberToReturn)
            : base(new BsonDocumentDescriptor())
        {
            Header = new MessageHeader(OpCode.GetMore);
            FullCollectionName = fullCollectionName;
            CursorId = cursorId;
            NumberToReturn = numberToReturn;
        }

        /// <summary>
        /// cursorID from the OP_REPLY.
        /// </summary>
        /// <value>The cursor id.</value>
        public long CursorId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the collection.
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { get; set; }

        /// <summary>
        /// Gets or sets the number to return.
        /// </summary>
        /// <value>The number to return.</value>
        public int NumberToReturn { get; set; }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer, 0);
            writer.Write(FullCollectionName, false);
            writer.WriteValue(BsonDataType.Integer, NumberToReturn);
            writer.WriteValue(BsonDataType.Long, CursorId);
        }

        /// <summary>
        /// Calculates the size of the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        protected override int CalculateBodySize(BsonWriter writer){
            var size = 4; //first int32
            size += writer.CalculateSize(FullCollectionName, false);
            size += 12; //number to return + cursorid
            return size;
        }
    }
}