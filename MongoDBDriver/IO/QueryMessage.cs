/*
 * User: scorder
 * Date: 7/7/2009
 */
using System;
using System.IO;
using System.Text;


using MongoDB.Driver.Bson;

namespace MongoDB.Driver.IO
{
    /// <summary>
    /// Description of QueryMessage.
    /// </summary>
    public class QueryMessage : RequestMessage
    {
//    MsgHeader header;                 // standard message header
//    int32     ZERO;                   // 0 - reserved for future use
//    cstring   fullCollectionName;     // "dbname.collectionname"
//    int32     numberToSkip;           // number of documents to skip when returning results
//    int32     numberToReturn;         // number of documents to return in the first OP_REPLY
//    BSON      query ;                 // query object.  See below for details.
//  [ BSON      returnFieldSelector; ]  // OPTIONAL : selector indicating the fields to return.  See below for details.
#region "Properties"
        private string fullCollectionName;      
        public string FullCollectionName {
            get { return fullCollectionName; }
            set { fullCollectionName = value; }
        }
        
        private Int32 numberToSkip;
        public int NumberToSkip {
            get { return numberToSkip; }
            set { numberToSkip = value; }
        }
        
        private Int32 numberToReturn;
        public int NumberToReturn {
            get { return numberToReturn; }
            set { numberToReturn = value; }
        }
        
        private BsonDocument query;
        public BsonDocument Query {
            get { return query; }
            set { query = value; }
        }

        private BsonDocument returnFieldSelector;       
        public BsonDocument ReturnFieldSelector {
            get { return returnFieldSelector; }
            set { returnFieldSelector = value; }
        }
        
#endregion
        
#region "Ctors"
        
        public QueryMessage(){
            this.Header = new MessageHeader(OpCode.Query);
        }
        
        public QueryMessage(BsonDocument query, String fullCollectionName)
            :this(query,fullCollectionName,0,0){
        }
        
        public QueryMessage(BsonDocument query, String fullCollectionName, Int32 numberToReturn, Int32 numberToSkip)
            :this(query,fullCollectionName,numberToReturn, numberToSkip, null){
        }
        
        public QueryMessage(BsonDocument query, String fullCollectionName, Int32 numberToReturn, 
                            Int32 numberToSkip, BsonDocument returnFieldSelector){
            this.Header = new MessageHeader(OpCode.Query);
            this.Query = query;
            this.FullCollectionName = fullCollectionName;
            this.NumberToReturn = numberToReturn;
            this.NumberToSkip = NumberToSkip;
            this.ReturnFieldSelector = returnFieldSelector;
        }
#endregion
        
        protected override void WriteBody (Stream stream){
            BsonWriter writer = new BsonWriter(stream);
            //TODO Implement Query Options (defaulting to none.
            writer.Write(0);
            writer.Write(this.FullCollectionName);
            writer.Write(this.numberToSkip);
            writer.Write(this.NumberToReturn);
            this.Query.Write(writer);
            if(this.ReturnFieldSelector != null) this.ReturnFieldSelector.Write(writer);
            
            writer.Flush();
        }
        

    }
}
