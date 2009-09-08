
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
        //    int32     upsert;             // value 0 or 1 for an 'upsert' operation
        //    BSON      selector;           // the query to select the document
        //    BSON      document;           // the document data to update with or insert
        //}
        private string fullCollectionName;  
        public string FullCollectionName {
            get { return fullCollectionName; }
            set { fullCollectionName = value; }
        }
        
        private BsonDocument selector;      
        public BsonDocument Selector {
            get { return selector; }
            set { selector = value; }
        }

        private BsonDocument document;      
        public BsonDocument Document {
            get { return document; }
            set { document = value; }
        }
        
        private int upsert;
        public int Upsert{
            get { return upsert; }
            set { upsert = value; }
        }
        
        public UpdateMessage(){
            this.Header = new MessageHeader(OpCode.Update);
        }
        
        protected override void WriteBody(Stream stream){
            BsonWriter writer = new BsonWriter(stream);
            writer.Write((int)0);
            writer.Write(this.FullCollectionName);
            writer.Write(this.Upsert);
            selector.Write(writer);
            document.Write(writer);
        }       
    }
}
