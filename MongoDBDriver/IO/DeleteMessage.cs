/*
 * User: scorder
 */using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    public class DeleteMessage : RequestMessageBase
    {
        //struct {
        //    MsgHeader header;                 // standard message header
        //    int32     ZERO;                   // 0 - reserved for future use
        //    cstring   fullCollectionName;     // "dbname.collectionname"
        //    int32     ZERO;                   // 0 - reserved for future use
        //    BSON      selector;               // query object.  See below for details.
        //}
        public string FullCollectionName { get; set; }

        public Document Selector { get; set; }

        public DeleteMessage(){
            this.Header = new MessageHeader(OpCode.Delete);
        }
        
        protected override void WriteBody (BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.WriteString(this.FullCollectionName);
            writer.WriteValue(BsonDataType.Integer,0);
            writer.Write(this.Selector);
        }
        
        protected override int CalculateBodySize(BsonWriter writer){
            int size = 8; //first int32, second int32 
            size += writer.CalculateSize(this.FullCollectionName,false);
            size += writer.CalculateSize(Selector);
            return size;
        }
    }
}