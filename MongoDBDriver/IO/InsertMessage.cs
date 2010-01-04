using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    /// <summary>
    /// Description of InsertMessage.
    /// </summary>
    public class InsertMessage : RequestMessage
    {

//      MsgHeader header;             // standard message header
//      int32     ZERO;               // 0 - reserved for future use
//      cstring   fullCollectionName; // "dbname.collectionname"
//      BSON[]    documents;          // one or more documents to insert into the collection

        private string fullCollectionName;
        public string FullCollectionName {
            get { return fullCollectionName; }
            set { fullCollectionName = value; }
        }
        
        private Document[] documents;
        public Document[] Documents {
            get { return documents; }
            set { documents = value; }
        }
        
        public InsertMessage(){
            this.Header = new MessageHeader(OpCode.Insert);
        }
            
        protected override void WriteBody (BsonWriter2 writer){
            writer.WriteValue(BsonDataType.Integer,0);
            writer.WriteString(this.FullCollectionName);
            
            foreach(Document doc in this.Documents){
                writer.WriteDocument(doc);
            }
        }
        
        protected override int CalculateBodySize(BsonWriter2 writer){
            int size = 4; //first int32
            size += writer.CalculateSize(this.FullCollectionName,false);
            foreach(Document doc in this.Documents){
                size += writer.CalculateSize(doc);
            }
            return size;
        }
    }
}
