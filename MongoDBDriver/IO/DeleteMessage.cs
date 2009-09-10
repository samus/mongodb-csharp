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
        
        private BsonDocument selector;      
        public BsonDocument Selector {
            get { return selector; }
            set { selector = value; }
        }
        
        public DeleteMessage(){
            this.Header = new MessageHeader(OpCode.Delete);
        }
        
        protected override void WriteBody(Stream stream){
            BsonWriter writer = new BsonWriter(stream);
            writer.Write((int)0);
            writer.Write(this.FullCollectionName);
            writer.Write((int)0);
            selector.Write(writer);
        }
        

    }
}
