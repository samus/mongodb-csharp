using MongoDB.Bson;

namespace MongoDB.Protocol
{
    /// <summary>
    /// The OP_UPDATE message is used to update a document in a collection.
    /// </summary>
    /// <remarks>
    /// struct {
    ///     MsgHeader header;             // standard message header
    ///     int32     ZERO;               // 0 - reserved for future use
    ///     cstring   fullCollectionName; // "dbname.collectionname"
    ///     int32     flags;              // bit vector. see below
    ///     BSON      selector;           // the query to select the document
    ///     BSON      document;           // the document data to update with or insert
    /// }
    /// </remarks>
    internal class UpdateMessage : RequestMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMessage"/> class.
        /// </summary>
        /// <param name="bsonWriterSettings">The bson writer settings.</param>
        public UpdateMessage(BsonWriterSettings bsonWriterSettings)
            : base(bsonWriterSettings){
            Header = new MessageHeader(OpCode.Update);
        }

        /// <summary>
        /// dbname.collectionname
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { get; set; }

        /// <summary>
        /// bit vector
        /// </summary>
        /// <value>The flags.</value>
        public int Flags { get; set; }

        /// <summary>
        /// The query to select the document.
        /// </summary>
        /// <value>The selector.</value>
        public object Selector { get; set; }

        /// <summary>
        /// The document data to update with or insert.
        /// </summary>
        /// <value>The document.</value>
        public object Document { get; set; }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonType.Integer, 0);
            writer.Write(FullCollectionName, false);
            writer.WriteValue(BsonType.Integer, Flags);
            writer.WriteObject(Selector);
            writer.WriteObject(Document);
        }

        /// <summary>
        /// Calculates the size of the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        protected override int CalculateBodySize(BsonWriter writer){
            var size = 4; //first int32
            size += writer.CalculateSize(FullCollectionName, false);
            size += 4; //flags
            size += writer.CalculateSizeObject(Selector);
            size += writer.CalculateSizeObject(Document);
            return size;
        }
    }
}