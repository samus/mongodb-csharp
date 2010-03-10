using System;
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
        private readonly BinaryReader _reader;
        private readonly Stream _stream;
        private readonly IBsonObjectBuilder _builder;

        public BsonReader(Stream stream)
            :this(stream,new DocumentBuilder()){
        }

        public BsonReader(Stream stream,IBsonObjectBuilder builder){
            _builder = builder;
            Position = 0;
            _stream = stream;
            _reader = new BinaryReader(_stream);
        }

        public int Position { get; private set; }

        public Document Read(){
            Position = 0;
            var doc = (Document)ReadObject();
            return doc;
        }

        public object ReadObject(){
            var instance = _builder.BeginObject();
            ReadElements(instance);
            return _builder.EndObject(instance);
        }

        public object ReadArray(){
            var instance = _builder.BeginArray();
            ReadElements(instance);
            return _builder.EndArray(instance);
        }

        public void ReadElements(object instance){
            var startPosition = Position;
            var size = _reader.ReadInt32();
            Position += 4;
            while((Position - startPosition) + 1 < size)
                ReadElement(instance);
            Position++;
            if(_reader.ReadByte() != 0)
                throw new InvalidDataException("Document not null terminated");
            if(size != Position - startPosition)
                throw new InvalidDataException(string.Format("Should have read {0} bytes from stream but only read {1}", size, (Position - startPosition)));
        }

        public void ReadElement(object instance){
            Position++;
            var typeNumber = (sbyte)_reader.ReadByte();
            var key = ReadString();
            _builder.BeginProperty(instance, key);
            var element = ReadElementType(typeNumber);
            _builder.EndProperty(instance, key, element);
        }

        private object ReadBinary(){
            var size = _reader.ReadInt32();
            Position += 4;
            var subtype = _reader.ReadByte();
            Position ++;
            if(subtype == (byte)Binary.TypeCode.General){
                size = _reader.ReadInt32();
                Position += 4;
            }
            var bytes = _reader.ReadBytes(size);
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

        private object ReadScope(){
            var startpos = Position;
            var size = _reader.ReadInt32();
            Position += 4;

            var val = ReadLengthString();
            var scope = (Document)ReadObject();
            if(size != Position - startpos)
                throw new InvalidDataException(string.Format("Should have read {0} bytes from stream but read {1} in CodeWScope",
                    size,
                    Position - startpos));
            
            return new CodeWScope(val, scope);
        }

        private object ReadCode(){
            return new Code{Value = ReadLengthString()};
        }

        private object ReadRegex(){
            return new MongoRegex{
                Expression = ReadString(), 
                Options = ReadString()
            };
        }

        public string ReadString(){
            EnsureBuffers();

            StringBuilder builder = null;

            do{
                var byteCount = 0;
                byte b;                
                while(byteCount < MaxCharBytesSize && (b = _reader.ReadByte()) > 0)
                    _byteBuffer[byteCount++] = b;

                Position += byteCount;

                var length = Encoding.UTF8.GetChars(_byteBuffer, 0, byteCount, _charBuffer, 0);

                if(byteCount < MaxCharBytesSize && builder == null){
                    Position++;
                    return new string(_charBuffer, 0, length);
                }

                if(builder == null)
                    builder = new StringBuilder(MaxCharBytesSize*2);

                builder.Append(_charBuffer, 0, length);

                if(byteCount >= MaxCharBytesSize)
                    continue;
                
                Position++;
                return builder.ToString();
            }
            while(true);
        }

        public string ReadLengthString(){
            var length = _reader.ReadInt32();
            var str = GetString(length - 1);
            _reader.ReadByte();

            Position += (4 + 1);
            return str;
        }

        private string GetString(int length){
            if(length == 0)
                return string.Empty;

            EnsureBuffers();

            StringBuilder builder = null;

            var totalBytesRead = 0;
            do{
                var count = ((length - totalBytesRead) > MaxCharBytesSize) ? MaxCharBytesSize : (length - totalBytesRead);
                var byteCount = _reader.BaseStream.Read(_byteBuffer, 0, count);
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
                    return _reader.ReadBoolean();
                case BsonDataType.Integer:
                    Position += 4;
                    return _reader.ReadInt32();
                case BsonDataType.Long:
                    Position += 8;
                    return _reader.ReadInt64();
                case BsonDataType.Date:
                    Position += 8;
                    var milliseconds = _reader.ReadInt64();
                    return BsonInfo.Epoch.AddMilliseconds(milliseconds);
                case BsonDataType.Oid:
                    Position += 12;
                    return new Oid(_reader.ReadBytes(12));
                case BsonDataType.Number:
                    Position += 8;
                    return _reader.ReadDouble();
                case BsonDataType.String:
                    return ReadLengthString();
                case BsonDataType.Obj:
                    return ReadObject();
                case BsonDataType.Array:
                    return ReadArray();
                case BsonDataType.Regex:
                    return ReadRegex();
                case BsonDataType.Code:
                    return ReadCode();
                case BsonDataType.CodeWScope:
                    return ReadScope();
                case BsonDataType.Binary:
                    return ReadBinary();
                default:
                    throw new ArgumentOutOfRangeException(String.Format("Type Number: {0} not recognized", typeNum));
            }
        }

        private void EnsureBuffers(){
            if(_byteBuffer == null)
                _byteBuffer = new byte[MaxCharBytesSize];
            if(_charBuffer != null)
                return;
            
            var charBufferSize = Encoding.UTF8.GetMaxCharCount(MaxCharBytesSize);
            _charBuffer = new char[charBufferSize];
        }
    }
}