using System;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// Description of QueryMessage.
    /// </summary>
    /// <remarks>
    ///    MsgHeader header;                 // standard message header
    ///    int32     opts;                   // query options.  See QueryOptions for values
    ///    cstring   fullCollectionName;     // "dbname.collectionname"
    ///    int32     numberToSkip;           // number of documents to skip when returning results
    ///    int32     numberToReturn;         // number of documents to return in the first OP_REPLY
    ///    BSON      query ;                 // query object.  See below for details.
    ///  [ BSON      returnFieldSelector; ]  // OPTIONAL : selector indicating the fields to return.  See below for details.
    /// </remarks>
    public class QueryMessage : RequestMessageBase
    {
        public QueryOptions Options { get; set; }

        public string FullCollectionName { get; set; }

        public int NumberToSkip { get; set; }

        public int NumberToReturn { get; set; }

        public Document Query { get; set; }

        public Document ReturnFieldSelector { get; set; }
        
        public QueryMessage(){
            this.Header = new MessageHeader(OpCode.Query);
        }
        
        public QueryMessage(Document query, String fullCollectionName)
            :this(query,fullCollectionName,0,0){
        }
        
        public QueryMessage(Document query, String fullCollectionName, Int32 numberToReturn, Int32 numberToSkip)
            :this(query,fullCollectionName,numberToReturn, numberToSkip, null){
        }
        
        public QueryMessage(Document query, String fullCollectionName, Int32 numberToReturn, 
                            Int32 numberToSkip, Document returnFieldSelector){
            this.Header = new MessageHeader(OpCode.Query);
            this.Query = query;
            this.FullCollectionName = fullCollectionName;
            this.NumberToReturn = numberToReturn;
            this.NumberToSkip = numberToSkip;
            this.ReturnFieldSelector = returnFieldSelector;
        }

        protected override void WriteBody (BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer,(int)this.Options);
            writer.Write(this.FullCollectionName,false);
            writer.WriteValue(BsonDataType.Integer,(int)this.NumberToSkip);
            writer.WriteValue(BsonDataType.Integer,(int)this.NumberToReturn);
            writer.WriteObject(this.Query);
            if(this.ReturnFieldSelector != null){
                writer.WriteObject(this.ReturnFieldSelector);
            }
        }
		
		protected override int CalculateBodySize(BsonWriter writer){
            int size = 12; //options, numbertoskip, numbertoreturn
            size += writer.CalculateSize(this.FullCollectionName,false);
            size += writer.CalculateSizeObject(this.Query);
            if(this.ReturnFieldSelector != null){
                size += writer.CalculateSizeObject(this.ReturnFieldSelector);
            }
            return size;
        }        
    }
}
