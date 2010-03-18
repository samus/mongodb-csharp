using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MongoDB.Driver.Serialization;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Reads binary streams containing BSON data and converts them to native types.
    /// </summary>
    public class BsonReader
    {
        private readonly BinaryReader _reader;
        private readonly Stream _stream;
        private readonly IBsonObjectBuilder _builder;

        private byte[] _byteBuffer;
        private char[] _charBuffer;

        private const int MaxCharBytesSize = 128;

        private byte[] seqRange1 = new byte[]{0,127};  //Range of 1-byte sequence
        private byte[] seqRange2 = new byte[]{194,223};//Range of 2-byte sequence
        private byte[] seqRange3 = new byte[]{224,239};//Range of 3-byte sequence
        private byte[] seqRange4 = new byte[]{240,244};//Range of 4-byte sequence

        public BsonReader(Stream stream)
            :this(stream,new ReflectionBuilder<Document>()){
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
        
        private void ReadElements(object instance){
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
        
        private void ReadElement(object instance){
            Position++;
            var typeNumber = (sbyte)_reader.ReadByte();
            var key = ReadString();
            _builder.BeginProperty(instance, key);
            var element = ReadElementType(typeNumber);
            _builder.EndProperty(instance, key, element);
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
        
        public string ReadString (){
            EnsureBuffers ();

            StringBuilder builder = new StringBuilder();
            int totalBytesRead = 0;
            int offset = 0;
            do {
                int byteCount = 0;
                int count = offset;
                byte b = 0;;
                while (count < MaxCharBytesSize && (b = _reader.ReadByte ()) > 0) {
                    _byteBuffer[count++] = b;
                }
                byteCount = count - offset;
                totalBytesRead += byteCount;
                Position += byteCount;
                
                if(count == 0) break; //first byte read was the terminator.
                int lastFullCharStop = GetLastFullCharStop(count - 1);
                
                int charCount = Encoding.UTF8.GetChars (_byteBuffer, 0, lastFullCharStop + 1, _charBuffer, 0);
                builder.Append (_charBuffer, 0, charCount);

                if(lastFullCharStop < byteCount - 1){
                    offset = byteCount - lastFullCharStop - 1;
                    //Copy end bytes to begining
                    Array.Copy(_byteBuffer, lastFullCharStop + 1, _byteBuffer, 0, offset);
                }else{
                    offset = 0;
                }
                
                if(b == 0){
                    break;
                }
            } while (true);
            Position++;
            return builder.ToString();

        }

        public string ReadLengthString (){
            int length = _reader.ReadInt32 ();
            string s = GetString (length - 1);
            _reader.ReadByte ();

            Position += (4 + 1);
            return s;
        }

        private string GetString (int length){
            if (length == 0)
                return string.Empty;

            EnsureBuffers ();

            StringBuilder builder = new StringBuilder (length);;
            
            int totalBytesRead = 0;
            int offset = 0;
            do {
                int byteCount = 0;                
                int count = ((length - totalBytesRead) > MaxCharBytesSize - offset) ? (MaxCharBytesSize - offset) :
                                                                            (length - totalBytesRead);
                
                byteCount = _reader.BaseStream.Read (_byteBuffer, offset, count);
                totalBytesRead += byteCount;
                byteCount += offset;
                
                int lastFullCharStop = 0;
                lastFullCharStop = GetLastFullCharStop(byteCount - 1);
                
                if (byteCount == 0)
                    throw new EndOfStreamException ("Unable to read beyond the end of the stream.");

                int charCount = Encoding.UTF8.GetChars (_byteBuffer, 0, lastFullCharStop + 1, _charBuffer, 0);
                builder.Append (_charBuffer, 0, charCount);
                
                if(lastFullCharStop < byteCount - 1){
                    offset = byteCount - lastFullCharStop - 1;
                    //Copy end bytes to begining
                    Array.Copy(_byteBuffer, lastFullCharStop + 1, _byteBuffer, 0, offset);
                }else{
                    offset = 0;
                }
                
            } while (totalBytesRead < length);
            
            Position += totalBytesRead;
            return builder.ToString ();
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
        
        private int GetLastFullCharStop(int start){
            int lookbackPos = start;
            int bis = 0;
            while(lookbackPos >= 0){
                bis = BytesInSequence(_byteBuffer[lookbackPos]);
                if(bis == 0){
                    lookbackPos--;
                    continue;
                }else if(bis == 1){
                    break;
                }else{
                    lookbackPos--;
                    break;
                }
            }
            if(bis == start - lookbackPos){
                //Full character.
                return start;
            }else{
                return lookbackPos;
            }
        }
        
        private int BytesInSequence(byte b){
            if(b <= seqRange1[1]) return 1;
            if(b >= seqRange2[0] && b <= seqRange2[1]) return 2;
            if(b >= seqRange3[0] && b <= seqRange3[1]) return 3;
            if(b >= seqRange4[0] && b <= seqRange4[1]) return 4;
            return 0;
        }
        
        private void EnsureBuffers (){
            if (_byteBuffer == null) {
                _byteBuffer = new byte[MaxCharBytesSize];
            }
            if (_charBuffer == null) {
                int charBufferSize = Encoding.UTF8.GetMaxCharCount (MaxCharBytesSize);
                _charBuffer = new char[charBufferSize];
            }
        }

        private Type GetTypeForIEnumerable (Document doc){
            if (doc.Keys.Count < 1)
                return typeof(Object);
            Type comp = null;
            foreach (String key in doc.Keys) {
                Object obj = doc[key];
                Type test = obj.GetType ();
                if (comp == null) {
                    comp = test;
                } else {
                    if (comp != test)
                        return typeof(Object);
                }
            }
            return comp;
        }

        private IEnumerable ConvertToArray (Document doc){
            var genericListType = typeof(List<>);
            var arrayType  = GetTypeForIEnumerable(doc);
            var listType = genericListType.MakeGenericType(arrayType);

            var list = (IList)Activator.CreateInstance(listType);
            
            foreach (String key in doc.Keys) {
                list.Add(doc[key]);
            }

            return list;
        }
    }
}