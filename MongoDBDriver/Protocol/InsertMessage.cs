using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// The OP_INSERT message is used to insert one or more documents into a collection.
    /// </summary>
    /// <remarks>
    /// struct {
    ///     MsgHeader header;             // standard message header
    ///     int32     ZERO;               // 0 - reserved for future use
    ///     cstring   fullCollectionName; // "dbname.collectionname"
    ///     BSON[]    documents;          // one or more documents to insert into the collection
    /// }
    /// </remarks>
    public class InsertMessage : MessageBase, IRequestMessage
    {
        private readonly IBsonObjectDescriptor _objectDescriptor;
        private readonly List<MessageChunk> _chunks = new List<MessageChunk>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertMessage"/> class.
        /// </summary>
        public InsertMessage(IBsonObjectDescriptor objectDescriptor){
            if(objectDescriptor == null)
                throw new ArgumentNullException("objectDescriptor");
            _objectDescriptor = objectDescriptor;
            Header = new MessageHeader(OpCode.Insert);
        }

        /// <summary>
        /// Gets or sets the full name of the collection.
        /// </summary>
        /// <value>The full name of the collection.</value>
        public string FullCollectionName { get; set; }

        /// <summary>
        /// Gets or sets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public object[] Documents { get; set; }

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Write(Stream stream){
            var bstream = new BufferedStream(stream);
            var bwriter = new BsonWriter(bstream, _objectDescriptor);

            ChunkMessage(bwriter);

            foreach(var chunk in _chunks)
                WriteChunk(bstream, chunk);
        }

        /// <summary>
        ///   Breaks down an insert message that may be too large into managable sizes.  
        ///   When inserting only one document there will be only one chunk.  However chances
        ///   are that when inserting thousands of documents at once there will be many.
        /// </summary>
        protected void ChunkMessage(BsonWriter writer){
            var baseSize = CalculateBaseSize(writer);

            var chunk = new MessageChunk{Size = baseSize, Documents = new List<object>()};
            
            foreach(var document in Documents){
                var documentSize = writer.CalculateSize(document);
                
                if(documentSize + baseSize >= MaximumMessageSize)
                    throw new MongoException("Document is too big to fit in a message.");

                if(documentSize + chunk.Size > MaximumMessageSize){
                    _chunks.Add(chunk);
                    chunk = new MessageChunk{Size = baseSize, Documents = new List<object>()};
                }
                
                chunk.Documents.Add(document);
                chunk.Size += documentSize;
            }

            _chunks.Add(chunk);
        }

        /// <summary>
        ///   The base size that all chunks will have.
        /// </summary>
        protected int CalculateBaseSize(BsonWriter writer){
            var size = 4; //first int32
            size += writer.CalculateSize(FullCollectionName, false);
            size += Header.MessageLength;
            return size;
        }

        /// <summary>
        ///   Writes out a header and the chunk of documents.
        /// </summary>
        /// <param name = "stream"></param>
        /// <param name = "chunk"></param>
        protected void WriteChunk(Stream stream, MessageChunk chunk){
            WriteHeader(new BinaryWriter(stream), chunk.Size);

            var writer = new BsonWriter(stream, new BsonDocumentDescriptor());
            writer.WriteValue(BsonDataType.Integer, 0);
            writer.Write(FullCollectionName, false);

            foreach(var document in chunk.Documents)
                writer.WriteObject(document);

            writer.Flush();
        }

        /// <summary>
        /// Writes the header.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="messageSize">Size of the MSG.</param>
        protected void WriteHeader(BinaryWriter writer, int messageSize){
            var header = Header;
            writer.Write(messageSize);
            writer.Write(header.RequestId);
            writer.Write(header.ResponseTo);
            writer.Write((int)header.OpCode);
            writer.Flush();
        }

        protected struct MessageChunk
        {
            public List<object> Documents;
            public int Size;
        }
    }
}