using MongoDB.Bson;

namespace MongoDB.Protocol
{
    /// <summary>
    /// The OP_DELETE message is used to remove one or more messages from a collection.
    /// </summary>
    /// <remarks>
    /// struct {
    ///     MsgHeader header;                 // standard message header
    ///     int32     ZERO;                   // 0 - reserved for future use
    ///     cstring   fullCollectionName;     // "dbname.collectionname"
    ///     int32     ZERO;                   // 0 - reserved for future use
    ///     BSON      selector;               // query object.  See below for details.
    /// }
    /// </remarks>
    public class DeleteMessage : RequestMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMessage"/> class.
        /// </summary>
        /// <param name="bsonWriterSettings">The bson writer settings.</param>
        public DeleteMessage(BsonWriterSettings bsonWriterSettings) 
            : base(bsonWriterSettings){
            Header = new MessageHeader(OpCode.Delete);
        }

        /// <summary>
        /// Gets or sets the full name of the collection.
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { get; set; }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>The selector.</value>
        public object Selector { get; set; }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonType.Integer, 0);
            writer.Write(FullCollectionName, false);
            writer.WriteValue(BsonType.Integer, 0);
            writer.WriteObject(Selector);
        }

        /// <summary>
        /// Calculates the size of the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        protected override int CalculateBodySize(BsonWriter writer){
            var size = 8; //first int32, second int32 
            size += writer.CalculateSize(FullCollectionName, false);
            size += writer.CalculateSizeObject(Selector);
            return size;
        }
    }
}