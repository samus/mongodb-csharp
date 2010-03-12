using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///   struct {
    ///   MsgHeader header;             // standard message header
    ///   int32     ZERO;               // 0 - reserved for future use
    ///   cstring   fullCollectionName; // "dbname.collectionname"
    ///   int32     flags;              // value 0 for upsert 1 for multiupdate operation
    ///   BSON      selector;           // the query to select the document
    ///   BSON      document;           // the document data to update with or insert
    ///   }
    /// </remarks>
    public class UpdateMessage : RequestMessageBase
    {
        public UpdateMessage(){
            Header = new MessageHeader(OpCode.Update);
        }

        public string FullCollectionName { get; set; }

        public Document Selector { get; set; }

        public Document Document { get; set; }

        public int Flags { get; set; }

        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer, 0);
            writer.Write(FullCollectionName, false);
            writer.WriteValue(BsonDataType.Integer, Flags);
            writer.WriteObject(Selector);
            writer.WriteObject(Document);
        }

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