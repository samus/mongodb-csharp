/*
 * User: scorder
 */
using System;
using System.IO;

using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    
    
    public class DeleteMessage : RequestMessage
    {
        //struct {
        //    MsgHeader header;                 // standard message header
        //    int32     ZERO;                   // 0 - reserved for future use
        //    cstring   fullCollectionName;     // "dbname.collectionname"
        //    int32     ZERO;                   // 0 - reserved for future use
        //    BSON      selector;               // query object.  See below for details.
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
        
        public DeleteMessage(){
            this.Header = new MessageHeader(OpCode.Delete);
        }
        
        protected override void WriteBody (BsonWriter2 writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.WriteString(this.FullCollectionName);
            writer.WriteValue(BsonDataType.Integer,0);
            writer.WriteDocument(this.Selector);
        }
        
        protected override int CalculateBodySize(BsonWriter2 writer){
            int size = 8; //first int32, second int32 
            size += writer.CalculateSize(this.FullCollectionName,false);
            size += writer.CalculateSize(selector);
            return size;
        }
    }
}