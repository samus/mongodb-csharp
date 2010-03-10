using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///  struct {
    ///       MsgHeader header;             // standard message header
    ///       int32     ZERO;               // 0 - reserved for future use
    ///       cstring   fullCollectionName; // "dbname.collectionname"
    ///       int32     flags;              // value 0 for upsert 1 for multiupdate operation
    ///       BSON      selector;           // the query to select the document
    ///       BSON      document;           // the document data to update with or insert
    ///  }
    /// </remarks>
    public class UpdateMessage : RequestMessageBase
    {
        public string FullCollectionName { get; set; }

        public Document Selector { get; set; }

        public Document Document { get; set; }

        public int Flags { get; set; }

        public UpdateMessage(){
            this.Header = new MessageHeader(OpCode.Update);
        }
        
        protected override void WriteBody (BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.Write(this.FullCollectionName,false);
            writer.WriteValue(BsonDataType.Integer,this.Flags);
            writer.WriteObject(Selector);
            writer.WriteObject(Document);
        }
        
        protected override int CalculateBodySize(BsonWriter writer){
            int size = 4; //first int32
            size += writer.CalculateSize(this.FullCollectionName,false);
            size += 4; //flags
            size += writer.CalculateSizeObject(this.Selector);
            size += writer.CalculateSizeObject(this.Document);
            return size;
        }       
    }
}