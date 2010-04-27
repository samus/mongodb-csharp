using System;
using MongoDB.Bson;

namespace MongoDB.Protocol
{
    /// <summary>
    /// The OP_QUERY message is used to query the database for documents in a collection.
    /// </summary>
    /// <remarks>
    /// struct {
    ///     MsgHeader header;                 // standard message header
    ///     int32     opts;                   // query options.  See below for details.
    ///     cstring   fullCollectionName;     // "dbname.collectionname"
    ///     int32     numberToSkip;           // number of documents to skip when returning results
    ///     int32     numberToReturn;         // number of documents to return in the first OP_REPLY
    ///     BSON      query ;                 // query object.  See below for details.
    ///     [ BSON      returnFieldSelector; ]  // OPTIONAL : selector indicating the fields to return.  See below for details.
    /// }
    /// </remarks>
    public class QueryMessage : RequestMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMessage"/> class.
        /// </summary>
        /// <param name="objectDescriptor">The object descriptor.</param>
        public QueryMessage(IBsonObjectDescriptor objectDescriptor)
            : base(objectDescriptor){
            Header = new MessageHeader(OpCode.Query);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMessage"/> class.
        /// </summary>
        /// <param name="objectDescriptor">The object descriptor.</param>
        /// <param name="query">The query.</param>
        /// <param name="fullCollectionName">Full name of the collection.</param>
        public QueryMessage(IBsonObjectDescriptor objectDescriptor, object query, String fullCollectionName)
            : this(objectDescriptor, query, fullCollectionName, 0, 0){
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMessage"/> class.
        /// </summary>
        /// <param name="objectDescriptor">The object descriptor.</param>
        /// <param name="query">The query.</param>
        /// <param name="fullCollectionName">Full name of the collection.</param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <param name="numberToSkip">The number to skip.</param>
        public QueryMessage(IBsonObjectDescriptor objectDescriptor, object query, String fullCollectionName, Int32 numberToReturn, Int32 numberToSkip)
            : this(objectDescriptor, query, fullCollectionName, numberToReturn, numberToSkip, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMessage"/> class.
        /// </summary>
        /// <param name="objectDescriptor">The object descriptor.</param>
        /// <param name="query">The query.</param>
        /// <param name="fullCollectionName">Full name of the collection.</param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <param name="numberToSkip">The number to skip.</param>
        /// <param name="returnFieldSelector">The return field selector.</param>
        public QueryMessage(IBsonObjectDescriptor objectDescriptor,
            object query,
            String fullCollectionName,
            Int32 numberToReturn,
            Int32 numberToSkip,
            object returnFieldSelector)
            : base(objectDescriptor)
        {
            Header = new MessageHeader(OpCode.Query);
            Query = query;
            FullCollectionName = fullCollectionName;
            NumberToReturn = numberToReturn;
            NumberToSkip = numberToSkip;
            ReturnFieldSelector = returnFieldSelector;
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public QueryOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the full name of the collection.
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { get; set; }

        /// <summary>
        /// Gets or sets the number to skip.
        /// </summary>
        /// <value>The number to skip.</value>
        public int NumberToSkip { get; set; }

        /// <summary>
        /// Gets or sets the number to return.
        /// </summary>
        /// <value>The number to return.</value>
        public int NumberToReturn { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public object Query { get; set; }

        /// <summary>
        /// Gets or sets the return field selector.
        /// </summary>
        /// <value>The return field selector.</value>
        public object ReturnFieldSelector { get; set; }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteBody(BsonWriter writer){
            writer.WriteValue(BsonDataType.Integer, (int)Options);
            writer.Write(FullCollectionName, false);
            writer.WriteValue(BsonDataType.Integer, NumberToSkip);
            writer.WriteValue(BsonDataType.Integer, NumberToReturn);
            writer.WriteObject(Query);
            if(ReturnFieldSelector != null)
                writer.WriteObject(ReturnFieldSelector);
        }

        /// <summary>
        /// Calculates the size of the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        protected override int CalculateBodySize(BsonWriter writer){
            var size = 12; //options, numbertoskip, numbertoreturn
            size += writer.CalculateSize(FullCollectionName, false);
            size += writer.CalculateSizeObject(Query);
            if(ReturnFieldSelector != null)
                size += writer.CalculateSizeObject(ReturnFieldSelector);
            return size;
        }
    }
}