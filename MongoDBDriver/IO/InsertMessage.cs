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
    public class InsertMessage : Message
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
        public void Write (Stream stream){
            //protected override void WriteBody(Stream stream){
            BufferedStream bstream = new BufferedStream(stream);
            BinaryWriter writer = new BinaryWriter(bstream);
            BsonWriter2 bwriter = new BsonWriter2(bstream);
            
            Header.MessageLength += this.CalculateBodySize(bwriter);
            
            writer.Write(Header.MessageLength);
            writer.Write(Header.RequestId);
            writer.Write(Header.ResponseTo);
            writer.Write((int)Header.OpCode);
            
            Debug.WriteLine(Header, "Insert Message");
            
            writer.Write((int)0);
            writer.Flush();
            bwriter.WriteString(this.FullCollectionName);
            
            foreach(Document doc in this.Documents){
                bwriter.WriteDocument(doc);
            }
            bwriter.Flush();
        }
        
        protected int CalculateBodySize(BsonWriter2 writer){
            int size = 4; //first int32
            size += writer.CalculateSize(this.FullCollectionName,false);
            foreach(Document doc in this.Documents){
                size += writer.CalculateSize(doc);
            }
            return size;
        }
    }
}
