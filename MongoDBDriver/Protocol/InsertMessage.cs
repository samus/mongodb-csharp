using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// Description of InsertMessage.
    /// </summary>
    /// <remarks>
    ///      MsgHeader header;             // standard message header
    ///      int32     ZERO;               // 0 - reserved for future use
    ///      cstring   fullCollectionName; // "dbname.collectionname"
    ///      BSON[]    documents;          // one or more documents to insert into the collection
    /// </remarks>
    public class InsertMessage : RequestMessageBase
    {
        public string FullCollectionName { get; set; }

        public Document[] Documents { get; set; }

        public InsertMessage(){
            this.Header = new MessageHeader(OpCode.Insert);
        }

        protected override void WriteBody (BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.Write(this.FullCollectionName,false);
            
            foreach(Document doc in this.Documents){
                writer.WriteObject(doc);
            }
        }
        
        protected override int CalculateBodySize(BsonWriter writer){
            int size = 4; //first int32
            size += writer.CalculateSize(this.FullCollectionName,false);
            foreach(Document doc in this.Documents){
                size += writer.CalculateSizeObject(doc);
            }
            return size;
        }
    }
}
