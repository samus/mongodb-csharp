/*
 * User: scorder
 */
using System;
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
        
        private BsonDocument[] bsonDocuments;       
        public BsonDocument[] BsonDocuments {
            get { return bsonDocuments; }
            set { bsonDocuments = value; }
        }
        
        public InsertMessage(){
            this.Header = new MessageHeader(OpCode.Insert);
        }
        
        protected override void WriteBody(Stream stream){
            BsonWriter writer = new BsonWriter(stream);
            writer.Write((int)0);
            writer.Write(this.FullCollectionName);
            foreach(BsonDocument bdoc in this.BsonDocuments){
                bdoc.Write(writer);
            }
        }
    }
}
