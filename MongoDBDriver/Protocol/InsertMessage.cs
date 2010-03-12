using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    ///   Description of InsertMessage.
    /// </summary>
    /// <remarks>
    ///   MsgHeader header;             // standard message header
    ///   int32     ZERO;               // 0 - reserved for future use
    ///   cstring   fullCollectionName; // "dbname.collectionname"
    ///   BSON[]    documents;          // one or more documents to insert into the collection
    /// </remarks>
    public class InsertMessage<T> : RequestMessageBase
        where T : class
    {
        public InsertMessage(){
            Header = new MessageHeader(OpCode.Insert);
        }

        public string FullCollectionName { get; set; }

        public T[] Documents { get; set; }

        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer, 0);
            writer.Write(FullCollectionName, false);

            foreach(var doc in Documents)
                writer.WriteObject(doc);
        }

        protected override int CalculateBodySize(BsonWriter writer){
            var size = 4; //first int32
            size += writer.CalculateSize(FullCollectionName, false);
            foreach(var doc in Documents)
                size += writer.CalculateSizeObject(doc);
            return size;
        }
    }
}