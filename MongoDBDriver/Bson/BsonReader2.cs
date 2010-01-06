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
		private byte[] _byteBuffer;
	    private char[] _charBuffer;

		private const int MaxCharBytesSize = 128;


		public BsonReader2 (Stream stream)
		{
			this.stream = stream;
			reader = new BinaryReader (this.stream);
		}

		public Document Read ()
		{
			Document doc = null;
			int read = ReadDocument (ref doc);
			return doc;
		}
		public int ReadDocument (ref Document doc)
		{
			doc = new Document ();
			int size = reader.ReadInt32 ();
			int bytesRead = 4;
			while (bytesRead + 1 < size) {
				bytesRead += ReadElement (doc);
			}
			byte eoo = reader.ReadByte ();
			bytesRead++;
			if (eoo != (byte)0)
				throw new System.IO.InvalidDataException ("Document not null terminated");
			if (size != bytesRead) {
				throw new System.IO.InvalidDataException (string.Format ("Should have read {0} bytes from stream but only read {1}", size, bytesRead));
			}
			return size;
		}

		public int ReadElement (Document doc)
		{
			sbyte typeNum = (sbyte)reader.ReadByte ();
			int read = 1;
			String key = ReadString ();
			read += encoding.GetByteCount (key) + 1;
			
			Object element = null;
			read += ReadElementType (typeNum, ref element);
			doc.Add (key, element);
			
			return read;
		}

		public int ReadElementType (sbyte typeNum, ref Object element)
		{
			int read = 0;
			switch ((BsonDataType)typeNum) {
			case BsonDataType.Null:
				element = MongoDBNull.Value;
				return 0;
			case BsonDataType.Boolean:
				element = reader.ReadBoolean ();
				return 1;
			case BsonDataType.Integer:
				element = reader.ReadInt32 ();
				return 4;
			case BsonDataType.Long:
				element = reader.ReadInt64 ();
				return 8;
			case BsonDataType.Date:
				long millis = reader.ReadInt64 ();
				element = epoch.AddMilliseconds (millis);
				return 8;
			case BsonDataType.Oid:
				element = new Oid (reader.ReadBytes (12));
				return 12;
			case BsonDataType.Number:
				element = reader.ReadDouble ();
				return 8;
			case BsonDataType.String:{
				element = ReadLenString ();
				return encoding.GetByteCount((String)element) + 5;
			}
			case BsonDataType.Obj:
				{
					Document doc = null;
					read = this.ReadDocument (ref doc);
					element = doc;
					return read;
				}

			case BsonDataType.Array:
				
				{
					Document doc = null;
					read = this.ReadDocument (ref doc);
					if (ElementsSameType (doc)) {
						element = ConvertToArray (doc);
					} else {
						element = doc;
					}
					return read;
				}

			case BsonDataType.Regex:
				
				{
					MongoRegex r = new MongoRegex ();
					r.Expression = this.ReadString ();
					read = encoding.GetByteCount (r.Expression) + 1;
					r.Options = this.ReadString ();
					read += encoding.GetByteCount (r.Options) + 1;
					element = r;
					return read;
				}

			case BsonDataType.Code:
				{
					Code c = new Code ();
					element = c;
					c.Value = ReadLenString();
					return encoding.GetByteCount(c.Value) + 5;
				}

			case BsonDataType.CodeWScope:
				{
					int size = reader.ReadInt32 ();
					read = 4;
					
					String val = ReadLenString();
					read += encoding.GetByteCount(val) + 5;
					Document scope = null;
					read += ReadDocument (ref scope);
					element = new CodeWScope (val, scope);
					if (size != read) {
						throw new System.IO.InvalidDataException (string.Format ("Should have read {0} bytes from stream but read {1} in CodeWScope", size, read));
					}
					return read;
				}

			case BsonDataType.Binary:
				
				{
					int size = reader.ReadInt32 ();
					int bytesRead = 4;
					byte subtype = reader.ReadByte ();
					bytesRead += 1;
					if (subtype == (byte)Binary.TypeCode.General) {
						size = reader.ReadInt32 ();
						bytesRead += 4;
					}
					byte[] bytes = reader.ReadBytes (size);
					bytesRead += size;
					Binary b = new Binary ();
					b.Bytes = bytes;
					b.Subtype = (Binary.TypeCode)subtype;
					element = b;
					return bytesRead;
				}

			default:
				throw new ArgumentOutOfRangeException (String.Format ("Type Number: {0} not recognized", typeNum));
			}
		}

		/// <summary>
		/// Reads a bson string from the stream and adds it to the current document.
		/// </summary>
		/// <param name="length">How much to read including null terminator</param>
		/// <returns></returns>
//		public int ReadLenString (ref Object val)
//		{
//			int length = reader.ReadInt32 ();
//			byte[] buff = reader.ReadBytes (length - 1);
//			reader.ReadByte ();
//			//ignore the terminator.
//			int read = 4 + buff.Length + 1;
//			val = encoding.GetString (buff);
//			
//			return read;
//		}

		/// <summary>
		/// Reads a string without a predetermined length.
		/// </summary>
		/// <returns></returns>
//		public String ReadString ()
//		{
//			StringBuilder sb = new StringBuilder ();
//			int buffsize = 64;
//			byte[] buff = new byte[buffsize];
//			int index = 0;
//			byte b = reader.ReadByte ();
//			while (b != 0) {
//				buff[index] = b;
//				index++;
//				if (index >= buffsize) {
//					sb.Append (encoding.GetString (buff));
//					buff = new byte[buffsize];
//					index = 0;
//				}
//				b = reader.ReadByte ();
//			}
//			sb.Append (encoding.GetString (buff, 0, index));
//			return sb.ToString ();
//		}

		public string ReadString ()
		{
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
				
				int length = Encoding.UTF8.GetChars (_byteBuffer, 0, byteCount, _charBuffer, 0);
				
				if (byteCount < MaxCharBytesSize && builder == null) {
					return new string (_charBuffer, 0, length);
				}
				
				if (builder == null)
					builder = new StringBuilder (MaxCharBytesSize * 2);
				
				builder.Append (_charBuffer, 0, length);
				
				if (byteCount < MaxCharBytesSize) {
					return builder.ToString ();
				}
			} while (true);
		}

		public string ReadLenString ()
		{
			int length = reader.ReadInt32 ();
			string s = GetString (length - 1);
			reader.ReadByte ();
			
			return s;
		}

		private string GetString (int length)
		{
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

		private void EnsureBuffers ()
		{
			if (_byteBuffer == null) {
				_byteBuffer = new byte[MaxCharBytesSize];
			}
			if (_charBuffer == null) {
				int charBufferSize = Encoding.UTF8.GetMaxCharCount (MaxCharBytesSize);
				_charBuffer = new char[charBufferSize];
			}
		}

		private bool ElementsSameType (Document doc)
		{
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

		private Object ConvertToArray (Document doc)
		{
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
