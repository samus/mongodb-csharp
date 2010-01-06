using System;
using System.IO;
using System.Text;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Reads binary streams containing BSON data and converts them to native types.
    /// </summary>
    public class BsonReader2
    {
        private static DateTime epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private Stream stream;
        private BinaryReader reader;
        private UTF8Encoding encoding = new UTF8Encoding ();
        private int position = 0;

        private byte[] _byteBuffer;
        private char[] _charBuffer;

        private const int MaxCharBytesSize = 128;


        public BsonReader2 (Stream stream)
        {
            this.stream = stream;
            reader = new BinaryReader (this.stream);
        }

        public int Position {
            get { return position; }
        }

        public Document Read ()
        {
            position = 0;
            Document doc = ReadDocument();
            return doc;
        }

        public Document ReadDocument(){
            int startpos = position;
            Document doc = new Document ();
            int size = reader.ReadInt32 ();
            position += 4;
            while ((position - startpos) + 1 < size) {
                ReadElement (doc);
            }
            byte eoo = reader.ReadByte ();
            position++;
            if (eoo != (byte)0)
                throw new System.IO.InvalidDataException ("Document not null terminated");
            if (size != position - startpos) {
                throw new System.IO.InvalidDataException (string.Format ("Should have read {0} bytes from stream but only read {1}", size, (position - startpos)));
            }
            return doc;
        }

        public void ReadElement (Document doc){
            sbyte typeNum = (sbyte)reader.ReadByte ();
            position++;
            String key = ReadString ();
            Object element = ReadElementType(typeNum);
            doc.Add (key, element);
        }

        public Object ReadElementType (sbyte typeNum){
            switch ((BsonDataType)typeNum) {
            case BsonDataType.Null:
                return MongoDBNull.Value;
            case BsonDataType.Boolean:
                position++;
                return reader.ReadBoolean ();
            case BsonDataType.Integer:
                position += 4;
                return reader.ReadInt32 ();
            case BsonDataType.Long:
                position += 8;
                return reader.ReadInt64 ();
            case BsonDataType.Date:
                position += 8;
                long millis = reader.ReadInt64 ();
                return epoch.AddMilliseconds (millis);
            case BsonDataType.Oid:
                position += 12;
                return new Oid (reader.ReadBytes (12));
            case BsonDataType.Number:
                position += 8;
                return reader.ReadDouble ();
            case BsonDataType.String:{
                return ReadLenString ();
            }
            case BsonDataType.Obj:{
                Document doc = this.ReadDocument();
                if(DBRef.IsDocumentDBRef(doc)){
                    return DBRef.FromDocument(doc);
                }
                return doc;
            }

            case BsonDataType.Array:{
                Document doc = this.ReadDocument();
                if (ElementsSameType (doc)) {
                    return ConvertToArray (doc);
                } else {
                    return doc;
                }
            }
            case BsonDataType.Regex:{
                MongoRegex r = new MongoRegex ();
                r.Expression = this.ReadString ();
                r.Options = this.ReadString ();
                return r;
            }
            case BsonDataType.Code:{
                Code c = new Code ();
                c.Value = ReadLenString();
                return c;
            }
            case BsonDataType.CodeWScope:{
                int startpos = position;
                int size = reader.ReadInt32 ();
                position += 4;

                String val = this.ReadLenString();
                Document scope = this.ReadDocument();
                if (size != position - startpos) {
                    throw new System.IO.InvalidDataException (string.Format ("Should have read {0} bytes from stream but read {1} in CodeWScope", size, position - startpos));
                }
                return new CodeWScope (val, scope);
            }
            case BsonDataType.Binary:{
                int size = reader.ReadInt32 ();
                position += 4;
                byte subtype = reader.ReadByte ();
                position ++;
                if (subtype == (byte)Binary.TypeCode.General) {
                    size = reader.ReadInt32 ();
                    position += 4;
                }
                byte[] bytes = reader.ReadBytes (size);
                position += size;
                Binary b = new Binary ();
                b.Bytes = bytes;
                b.Subtype = (Binary.TypeCode)subtype;
                return b;
            }
            default:
                throw new ArgumentOutOfRangeException (String.Format ("Type Number: {0} not recognized", typeNum));
            }
        }

        public string ReadString (){
            EnsureBuffers ();

            StringBuilder builder = null;

            int totalBytesRead = 0;
            do {
                int byteCount = 0;
                byte b;
                while (byteCount < MaxCharBytesSize && (b = reader.ReadByte ()) > 0) {
                    _byteBuffer[byteCount++] = b;
                }
                totalBytesRead += byteCount;
                position += byteCount;

                int length = Encoding.UTF8.GetChars (_byteBuffer, 0, byteCount, _charBuffer, 0);

                if (byteCount < MaxCharBytesSize && builder == null) {
                    position++;
                    return new string (_charBuffer, 0, length);
                }

                if (builder == null)
                    builder = new StringBuilder (MaxCharBytesSize * 2);

                builder.Append (_charBuffer, 0, length);

                if (byteCount < MaxCharBytesSize) {
                    position++;
                    return builder.ToString ();
                }
            } while (true);
        }

        public string ReadLenString (){
            int length = reader.ReadInt32 ();
            string s = GetString (length - 1);
            reader.ReadByte ();

            position += (4 + 1);
            return s;
        }

        private string GetString (int length){
            if (length == 0)
                return string.Empty;

            EnsureBuffers ();

            StringBuilder builder = null;

            int totalBytesRead = 0;
            do {
                int count = ((length - totalBytesRead) > MaxCharBytesSize) ? MaxCharBytesSize : (length - totalBytesRead);
                int byteCount = reader.BaseStream.Read (_byteBuffer, 0, count);
                if (byteCount == 0)
                    throw new EndOfStreamException ("Unable to read beyond the end of the stream.");

                position += byteCount;

                int charCount = Encoding.UTF8.GetChars (_byteBuffer, 0, byteCount, _charBuffer, 0);
                if (totalBytesRead == 0 && byteCount == length)
                    return new string (_charBuffer, 0, charCount);

                if (builder == null)
                    builder = new StringBuilder (length);

                builder.Append (_charBuffer, 0, charCount);
                totalBytesRead += byteCount;
            } while (totalBytesRead < length);

            return builder.ToString ();
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

        private bool ElementsSameType (Document doc){
            if (doc.Keys.Count < 1)
                return false;
            Type comp = null;
            foreach (String key in doc.Keys) {
                Object obj = doc[key];
                Type test = obj.GetType ();
                if (comp == null) {
                    comp = test;
                } else {
                    if (comp != test)
                        return false;
                }
            }
            return true;
        }

        private Object ConvertToArray (Document doc){
            Type arrayType = null;
            Array ret = null;
            int idx = 0;
            foreach (String key in doc.Keys) {
                if (ret == null) {
                    int length = doc.Keys.Count;
                    arrayType = doc[key].GetType ();
                    ret = Array.CreateInstance (arrayType, length);
                }
                ret.SetValue (doc[key], idx);
                idx++;
            }
            return ret;
        }
    }
}
