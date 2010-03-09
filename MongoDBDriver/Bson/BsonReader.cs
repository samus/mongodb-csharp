using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    ///   Reads binary streams containing BSON data and converts them to native types.
    /// </summary>
    public class BsonReader
    {
        private const int MaxCharBytesSize = 128;
        private byte[] _byteBuffer;
        private char[] _charBuffer;
        private readonly BinaryReader reader;
        private readonly Stream stream;

        public BsonReader(Stream stream){
            Position = 0;
            this.stream = stream;
            reader = new BinaryReader(this.stream);
        }

        public int Position { get; private set; }

        public Document Read(){
            Position = 0;
            var doc = ReadDocument();
            return doc;
        }

        public Document ReadDocument(){
            var startpos = Position;
            var doc = new Document();
            var size = reader.ReadInt32();
            Position += 4;
            while((Position - startpos) + 1 < size)
                ReadElement(doc);
            var eoo = reader.ReadByte();
            Position++;
            if(eoo != 0)
                throw new InvalidDataException("Document not null terminated");
            if(size != Position - startpos)
                throw new InvalidDataException(string.Format("Should have read {0} bytes from stream but only read {1}", size, (Position - startpos)));
            return doc;
        }

        public void ReadElement(Document doc){
            var typeNum = (sbyte)reader.ReadByte();
            Position++;
            var key = ReadString();
            var element = ReadElementType(typeNum);
            doc.Add(key, element);
        }

        public Object ReadElementType(sbyte typeNum){
            switch((BsonDataType)typeNum){
                case BsonDataType.Null:
                case BsonDataType.Undefined:
                    return DBNull.Value;
                case BsonDataType.MinKey:
                    return MongoMinKey.Value;
                case BsonDataType.MaxKey:
                    return MongoMaxKey.Value;
                case BsonDataType.Boolean:
                    Position++;
                    return reader.ReadBoolean();
                case BsonDataType.Integer:
                    Position += 4;
                    return reader.ReadInt32();
                case BsonDataType.Long:
                    Position += 8;
                    return reader.ReadInt64();
                case BsonDataType.Date:
                    Position += 8;
                    var millis = reader.ReadInt64();
                    return BsonInfo.Epoch.AddMilliseconds(millis);
                case BsonDataType.Oid:
                    Position += 12;
                    return new Oid(reader.ReadBytes(12));
                case BsonDataType.Number:
                    Position += 8;
                    return reader.ReadDouble();
                case BsonDataType.String:{
                    return ReadLengthString();
                }
                case BsonDataType.Obj:{
                    var doc = ReadDocument();
                    if(DBRef.IsDocumentDBRef(doc))
                        return DBRef.FromDocument(doc);
                    return doc;
                }

                case BsonDataType.Array:{
                    var doc = ReadDocument();
                    return ConvertToArray(doc);
                }
                case BsonDataType.Regex:{
                    return new MongoRegex{
                        Expression = ReadString(), 
                        Options = ReadString()
                    };
                }
                case BsonDataType.Code:{
                    return new Code{Value = ReadLengthString()};
                }
                case BsonDataType.CodeWScope:{
                    var startpos = Position;
                    var size = reader.ReadInt32();
                    Position += 4;

                    var val = ReadLengthString();
                    var scope = ReadDocument();
                    if(size != Position - startpos)
                        throw new InvalidDataException(string.Format("Should have read {0} bytes from stream but read {1} in CodeWScope",
                            size,
                            Position - startpos));
                    return new CodeWScope(val, scope);
                }
                case BsonDataType.Binary:{
                    var size = reader.ReadInt32();
                    Position += 4;
                    var subtype = reader.ReadByte();
                    Position ++;
                    if(subtype == (byte)Binary.TypeCode.General){
                        size = reader.ReadInt32();
                        Position += 4;
                    }
                    var bytes = reader.ReadBytes(size);
                    Position += size;

                    // From http://en.wikipedia.org/wiki/Universally_Unique_Identifier
                    // The most widespread use of this standard is in Microsoft's Globally Unique Identifiers (GUIDs).
                    if(subtype == 3 && 16 == size)
                        return new Guid(bytes);

                    return new Binary{
                        Bytes = bytes, 
                        Subtype = (Binary.TypeCode)subtype
                    };
                }
                default:
                    throw new ArgumentOutOfRangeException(String.Format("Type Number: {0} not recognized", typeNum));
            }
        }

        public string ReadString(){
            EnsureBuffers();

            StringBuilder builder = null;

            var totalBytesRead = 0;
            do{
                var byteCount = 0;
                byte b;
                while(byteCount < MaxCharBytesSize && (b = reader.ReadByte()) > 0)
                    _byteBuffer[byteCount++] = b;
                totalBytesRead += byteCount;
                Position += byteCount;

                var length = Encoding.UTF8.GetChars(_byteBuffer, 0, byteCount, _charBuffer, 0);

                if(byteCount < MaxCharBytesSize && builder == null){
                    Position++;
                    return new string(_charBuffer, 0, length);
                }

                if(builder == null)
                    builder = new StringBuilder(MaxCharBytesSize*2);

                builder.Append(_charBuffer, 0, length);

                if(byteCount < MaxCharBytesSize){
                    Position++;
                    return builder.ToString();
                }
            }
            while(true);
        }

        public string ReadLengthString(){
            var length = reader.ReadInt32();
            var s = GetString(length - 1);
            reader.ReadByte();

            Position += (4 + 1);
            return s;
        }

        private string GetString(int length){
            if(length == 0)
                return string.Empty;

            EnsureBuffers();

            StringBuilder builder = null;

            var totalBytesRead = 0;
            do{
                var count = ((length - totalBytesRead) > MaxCharBytesSize) ? MaxCharBytesSize : (length - totalBytesRead);
                var byteCount = reader.BaseStream.Read(_byteBuffer, 0, count);
                if(byteCount == 0)
                    throw new EndOfStreamException("Unable to read beyond the end of the stream.");

                Position += byteCount;

                var charCount = Encoding.UTF8.GetChars(_byteBuffer, 0, byteCount, _charBuffer, 0);
                if(totalBytesRead == 0 && byteCount == length)
                    return new string(_charBuffer, 0, charCount);

                if(builder == null)
                    builder = new StringBuilder(length);

                builder.Append(_charBuffer, 0, charCount);
                totalBytesRead += byteCount;
            }
            while(totalBytesRead < length);

            return builder.ToString();
        }

        private void EnsureBuffers(){
            if(_byteBuffer == null)
                _byteBuffer = new byte[MaxCharBytesSize];
            if(_charBuffer == null){
                var charBufferSize = Encoding.UTF8.GetMaxCharCount(MaxCharBytesSize);
                _charBuffer = new char[charBufferSize];
            }
        }

        private Type GetTypeForIEnumerable(Document doc){
            if(doc.Keys.Count < 1)
                return typeof(Object);
            Type comp = null;
            foreach(String key in doc.Keys){
                var obj = doc[key];
                var test = obj.GetType();
                if(comp == null)
                    comp = test;
                else if(comp != test)
                    return typeof(Object);
            }
            return comp;
        }

        private IEnumerable ConvertToArray(Document doc){
            var genericListType = typeof(List<>);
            var arrayType = GetTypeForIEnumerable(doc);
            var listType = genericListType.MakeGenericType(arrayType);

            var list = (IList)Activator.CreateInstance(listType);

            foreach(String key in doc.Keys)
                list.Add(doc[key]);

            return list;
        }
    }
}