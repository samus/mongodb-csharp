using System;
using System.IO;

using MongoDB.Driver;
using MongoDB.Driver.Bson;


namespace MongoDB.Driver.IO
{
    
    
    public class UpdateMessage : RequestMessage
    {
        //struct {
        //    MsgHeader header;             // standard message header
        //    int32     ZERO;               // 0 - reserved for future use
        //    cstring   fullCollectionName; // "dbname.collectionname"
        //    int32     flags;              // value 0 for upsert 1 for multiupdate operation
        //    BSON      selector;           // the query to select the document
        //    BSON      document;           // the document data to update with or insert
        //}
        private string fullCollectionName;  
        public string FullCollectionName {
            get { return fullCollectionName; }
            set { fullCollectionName = value; }
        }
        
        private Document selector;      
        public Document Selector {
            get { return selector; }
            set { selector = value; }
        }

        private Document document;      
        public Document Document {
            get { return document; }
            set { document = value; }
        }
        
        private int flags;
        public int Flags{
            get { return flags; }
            set { flags = value; }
        }
        
        public UpdateMessage(){
            this.Header = new MessageHeader(OpCode.Update);
        }
        
        protected override void WriteBody (BsonWriter2 writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.WriteString(this.FullCollectionName);
            writer.WriteValue(BsonDataType.Integer,this.Flags);
            writer.WriteDocument(selector);
            writer.WriteDocument(Document);
        }
        
        protected override int CalculateBodySize(BsonWriter2 writer){
            int size = 4; //first int32
            size += writer.CalculateSize(this.FullCollectionName,false);
            size += 4; //flags
            size += writer.CalculateSize(this.Selector);
            size += writer.CalculateSize(this.Document);
            return size;
        }       
    }
}