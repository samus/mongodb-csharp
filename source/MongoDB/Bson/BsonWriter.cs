using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MongoDB.Bson
{
    /// <summary>
    ///   Class that knows how to format a native object into bson bits.
    /// </summary>
    public class BsonWriter
    {
        private const int BufferLength = 256;
        private readonly byte[] _buffer;
        private readonly IBsonObjectDescriptor _descriptor;
        private readonly int _maxChars;
        private readonly Stream _stream;
        private readonly BinaryWriter _writer;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BsonWriter" /> class.
        /// </summary>
        /// <param name = "stream">The stream.</param>
        /// <param name = "settings">The settings.</param>
        public BsonWriter(Stream stream, BsonWriterSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException("settings");
            _stream = stream;
            _descriptor = settings.Descriptor;
            _writer = new BinaryWriter(_stream);
            _buffer = new byte[BufferLength];
            _maxChars = BufferLength/Encoding.UTF8.GetMaxByteCount(1);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BsonWriter" /> class.
        /// </summary>
        /// <param name = "stream">The stream.</param>
        /// <param name = "descriptor">The descriptor.</param>
        public BsonWriter(Stream stream, IBsonObjectDescriptor descriptor)
        {
            _stream = stream;
            _descriptor = descriptor;
            _writer = new BinaryWriter(_stream);
            _buffer = new byte[BufferLength];
            _maxChars = BufferLength/Encoding.UTF8.GetMaxByteCount(1);
        }

        /// <summary>
        ///   Writes the value.
        /// </summary>
        /// <param name = "type">Type of the data.</param>
        /// <param name = "obj">The obj.</param>
        public void WriteValue(BsonType type, Object obj)
        {
            switch(type)
            {
                case BsonType.MinKey:
                case BsonType.MaxKey:
                case BsonType.Null:
                    return;
                case BsonType.Boolean:
                    _writer.Write((bool)obj);
                    return;
                case BsonType.Integer:
                    _writer.Write((int)obj);
                    return;
                case BsonType.Long:
                    if(obj is TimeSpan)
                        _writer.Write(((TimeSpan)obj).Ticks);
                    else
                        _writer.Write((long)obj);
                    return;
                case BsonType.Timestamp:
                    _writer.Write(((MongoTimestamp)obj).Value);
                    return;
                case BsonType.Date:
                    Write((DateTime)obj);
                    return;
                case BsonType.Oid:
                    Write((Oid)obj);
                    return;
                case BsonType.Number:
                    _writer.Write(Convert.ToDouble(obj));
                    return;
                case BsonType.String:
                    if(obj is string)
                        Write((string)obj);
                    else
                        Write(obj.ToString());
                    return;
                case BsonType.Obj:
                    if(obj is DBRef)
                        Write((DBRef)obj);
                    else
                        WriteObject(obj);
                    return;
                case BsonType.Array:
                    WriteArray((IEnumerable)obj);
                    return;
                case BsonType.Regex:
                    if(obj is Regex)
                        Write(new MongoRegex((Regex)obj));
                    else
                        Write((MongoRegex)obj);
                    return;
                case BsonType.Code:
                    Write((Code)obj);
                    return;
                case BsonType.Symbol:
                    WriteValue(BsonType.String, ((MongoSymbol)obj).Value);
                    return;
                case BsonType.CodeWScope:
                    Write((CodeWScope)obj);
                    return;
                case BsonType.Binary:
                {
                    if(obj is Guid)
                        Write((Guid)obj);
                    else if(obj is byte[])
                        Write((byte[])obj);
                    else
                        Write((Binary)obj);
                    return;
                }
                default:
                    throw new NotImplementedException(String.Format("Writing {0} types not implemented.", obj.GetType().Name));
            }
        }

        /// <summary>
        ///   Writes the specified id.
        /// </summary>
        /// <param name = "id">The id.</param>
        private void Write(Oid id)
        {
            _writer.Write(id.ToByteArray());
        }

        /// <summary>
        ///   Writes the specified binary.
        /// </summary>
        /// <param name = "binary">The binary.</param>
        private void Write(Binary binary)
        {
            if(binary.Subtype == BinarySubtype.General)
            {
                _writer.Write(binary.Bytes.Length + 4);
                _writer.Write((byte)binary.Subtype);
                _writer.Write(binary.Bytes.Length);
            }
            else
            {
                _writer.Write(binary.Bytes.Length);
                _writer.Write((byte)binary.Subtype);
            }
            _writer.Write(binary.Bytes);
        }

        /// <summary>
        ///   Writes the specified GUID.
        /// </summary>
        /// <param name = "guid">The GUID.</param>
        private void Write(Guid guid)
        {
            _writer.Write(16);
            _writer.Write((byte)3);
            _writer.Write(guid.ToByteArray());
        }

        /// <summary>
        ///   Writes the specified bytes.
        /// </summary>
        /// <param name = "bytes">The bytes.</param>
        private void Write(byte[] bytes)
        {
            Write(new Binary(bytes));
        }

        /// <summary>
        ///   Writes the specified code scope.
        /// </summary>
        /// <param name = "codeScope">The code scope.</param>
        private void Write(CodeWScope codeScope)
        {
            _writer.Write(CalculateSize(codeScope));
            WriteValue(BsonType.String, codeScope.Value);
            WriteValue(BsonType.Obj, codeScope.Scope);
        }

        /// <summary>
        ///   Writes the specified code.
        /// </summary>
        /// <param name = "code">The code.</param>
        private void Write(Code code)
        {
            WriteValue(BsonType.String, code.Value);
        }

        /// <summary>
        ///   Writes the specified regex.
        /// </summary>
        /// <param name = "regex">The regex.</param>
        private void Write(MongoRegex regex)
        {
            Write(regex.Expression, false);
            Write(regex.RawOptions, false);
        }

        /// <summary>
        ///   Writes the specified reference.
        /// </summary>
        /// <param name = "reference">The reference.</param>
        public void Write(DBRef reference)
        {
            WriteObject((Document)reference);
        }

        /// <summary>
        ///   Writes the specified data time.
        /// </summary>
        /// <param name = "dateTime">The data time.</param>
        private void Write(DateTime dateTime)
        {
            var diff = dateTime.ToUniversalTime() - BsonInfo.Epoch;
            var time = Math.Floor(diff.TotalMilliseconds);
            _writer.Write((long)time);
        }

        /// <summary>
        ///   Writes the object.
        /// </summary>
        /// <param name = "obj">The obj.</param>
        public void WriteObject(object obj)
        {
            obj = _descriptor.BeginObject(obj);
            WriteElements(obj);
            _descriptor.EndObject(obj);
        }

        /// <summary>
        ///   Writes the elements.
        /// </summary>
        /// <param name = "obj">The obj.</param>
        private void WriteElements(object obj)
        {
            var properties = _descriptor.GetProperties(obj);
            var size = CalculateSizeObject(obj, properties);
            if(size >= BsonInfo.MaxDocumentSize)
                throw new ArgumentException("Maximum document size exceeded.");
            _writer.Write(size);
            foreach(var property in properties)
            {
                _descriptor.BeginProperty(obj, property);
                var bsonType = TranslateToBsonType(property.Value);
                _writer.Write((byte)bsonType);
                Write(property.Name, false);
                WriteValue(bsonType, property.Value);
                _descriptor.EndProperty(obj, property);
            }
            _writer.Write((byte)0);
        }

        /// <summary>
        ///   Writes the array.
        /// </summary>
        /// <param name = "enumerable">The enumerable.</param>
        public void WriteArray(IEnumerable enumerable)
        {
            var obj = _descriptor.BeginArray(enumerable);
            WriteElements(obj);
            _descriptor.EndArray(obj);
        }

        /// <summary>
        ///   Writes the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        private void Write(string value)
        {
            Write(value, true);
        }

        /// <summary>
        ///   Writes the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "includeLength">if set to <c>true</c> [include length].</param>
        public void Write(string value, bool includeLength)
        {
            if(includeLength)
                _writer.Write(CalculateSize(value, false));
            var byteCount = Encoding.UTF8.GetByteCount(value);
            if(byteCount < BufferLength)
            {
                Encoding.UTF8.GetBytes(value, 0, value.Length, _buffer, 0);
                _writer.Write(_buffer, 0, byteCount);
            }
            else
            {
                int charCount;
                var totalCharsWritten = 0;

                for(var i = value.Length; i > 0; i -= charCount)
                {
                    charCount = (i > _maxChars) ? _maxChars : i;
                    var count = Encoding.UTF8.GetBytes(value, totalCharsWritten, charCount, _buffer, 0);
                    _writer.Write(_buffer, 0, count);
                    totalCharsWritten += charCount;
                }
            }
            _writer.Write((byte)0);
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "obj">The obj.</param>
        /// <returns></returns>
        public int CalculateSize(Object obj)
        {
            if(obj == null)
                return 0;

            switch(TranslateToBsonType(obj))
            {
                case BsonType.MinKey:
                case BsonType.MaxKey:
                case BsonType.Null:
                    return 0;
                case BsonType.Boolean:
                    return 1;
                case BsonType.Integer:
                    return 4;
                case BsonType.Long:
                case BsonType.Date:
                case BsonType.Timestamp:
                    return 8;
                case BsonType.Oid:
                    return 12;
                case BsonType.Number:
                    return sizeof(Double);
                case BsonType.String:
                    if(obj is string)
                        return CalculateSize((string)obj);
                    return CalculateSize(obj.ToString());
                case BsonType.Obj:
                    return obj.GetType() == typeof(DBRef) ? CalculateSize((DBRef)obj) : CalculateSizeObject(obj);
                case BsonType.Array:
                    return CalculateSize((IEnumerable)obj);
                case BsonType.Regex:
                    if(obj is Regex)
                        return CalculateSize(new MongoRegex((Regex)obj));
                    return CalculateSize((MongoRegex)obj);
                case BsonType.Code:
                    return CalculateSize((Code)obj);
                case BsonType.CodeWScope:
                    return CalculateSize((CodeWScope)obj);
                case BsonType.Binary:
                {
                    if(obj is Guid)
                        return CalculateSize((Guid)obj);
                    if(obj is byte[])
                        return CalculateSize((byte[])obj);

                    return CalculateSize((Binary)obj);
                }
                case BsonType.Symbol:
                    return CalculateSize(((MongoSymbol)obj).Value, true);
            }

            throw new NotImplementedException(String.Format("Calculating size of {0} is not implemented.", obj.GetType().Name));
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "code">The code.</param>
        /// <returns></returns>
        private int CalculateSize(Code code)
        {
            return CalculateSize(code.Value, true);
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "regex">The regex.</param>
        /// <returns></returns>
        public int CalculateSize(MongoRegex regex)
        {
            var size = CalculateSize(regex.Expression, false);
            size += CalculateSize(regex.RawOptions, false);
            return size;
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "codeScope">The code scope.</param>
        /// <returns></returns>
        public int CalculateSize(CodeWScope codeScope)
        {
            var size = 4;
            size += CalculateSize(codeScope.Value, true);
            size += CalculateSizeObject(codeScope.Scope);
            return size;
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "binary">The binary.</param>
        /// <returns></returns>
        public int CalculateSize(Binary binary)
        {
            var size = 4; //size int
            size += 1; //subtype
            if(binary.Subtype == BinarySubtype.General)
                size += 4; //embedded size int
            size += binary.Bytes.Length;
            return size;
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "bytes">The bytes.</param>
        /// <returns></returns>
        public int CalculateSize(byte[] bytes)
        {
            return CalculateSize(new Binary(bytes));
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "guid">The GUID.</param>
        /// <returns></returns>
        public int CalculateSize(Guid guid)
        {
            return 21;
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "reference">The reference.</param>
        /// <returns></returns>
        public int CalculateSize(DBRef reference)
        {
            return CalculateSizeObject((Document)reference);
        }

        /// <summary>
        ///   Calculates the size object.
        /// </summary>
        /// <param name = "obj">The obj.</param>
        /// <returns></returns>
        public int CalculateSizeObject(object obj)
        {
            obj = _descriptor.BeginObject(obj);
            var properties = _descriptor.GetProperties(obj);

            var size = CalculateSizeObject(obj, properties);

            _descriptor.EndObject(obj);

            return size;
        }

        /// <summary>
        ///   Calculates the size object.
        /// </summary>
        /// <param name = "obj">The obj.</param>
        /// <param name = "propertys">The propertys.</param>
        /// <returns></returns>
        private int CalculateSizeObject(object obj, IEnumerable<BsonProperty> propertys)
        {
            var size = 4;
            foreach(var property in propertys)
            {
                var elsize = 1; //type
                _descriptor.BeginProperty(obj, property);
                elsize += CalculateSize(property.Name, false);
                elsize += CalculateSize(property.Value);
                _descriptor.EndProperty(obj, property);
                size += elsize;
            }
            size += 1; //terminator
            return size;
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "enumerable">The enumerable.</param>
        /// <returns></returns>
        public int CalculateSize(IEnumerable enumerable)
        {
            var obj = _descriptor.BeginArray(enumerable);
            var properties = _descriptor.GetProperties(obj);

            var size = CalculateSizeObject(obj, properties);

            _descriptor.EndArray(obj);

            return size;
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        public int CalculateSize(String value)
        {
            return CalculateSize(value, true);
        }

        /// <summary>
        ///   Calculates the size.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "includeLength">if set to <c>true</c> [include length].</param>
        /// <returns></returns>
        public int CalculateSize(String value, bool includeLength)
        {
            var size = 1; //terminator
            if(includeLength)
                size += 4;
            if(value != null)
                size += Encoding.UTF8.GetByteCount(value);
            return size;
        }

        /// <summary>
        ///   Flushes this instance.
        /// </summary>
        public void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        ///   Translates the type of to bson.
        /// </summary>
        /// <param name = "obj">The obj.</param>
        /// <returns></returns>
        protected BsonType TranslateToBsonType(object obj)
        {
            //TODO:Convert to use a dictionary
            if(obj == null)
                return BsonType.Null;

            var type = obj.GetType();

            if(obj is Enum) //special case enums               
                type = Enum.GetUnderlyingType(type);
            
            if(type == typeof(Double))
                return BsonType.Number;
            if(type == typeof(Single))
                return BsonType.Number;
            if(type == typeof(String))
                return BsonType.String;
            if(type == typeof(Uri))
                return BsonType.String;
            if(type == typeof(int))
                return BsonType.Integer;
            if(type == typeof(long))
                return BsonType.Long;
            if(type == typeof(MongoTimestamp))
                return BsonType.Timestamp;
            if(type == typeof(bool))
                return BsonType.Boolean;
            if(type == typeof(Oid))
                return BsonType.Oid;
            if(type == typeof(DateTime))
                return BsonType.Date;
            if(type == typeof(TimeSpan))
                return BsonType.Long;
            if(type == typeof(MongoRegex))
                return BsonType.Regex;
            if(type == typeof(Regex))
                return BsonType.Regex;
            if(type == typeof(DBRef))
                return BsonType.Obj;
            if(type == typeof(Code))
                return BsonType.Code;
            if(type == typeof(CodeWScope))
                return BsonType.CodeWScope;
            if(type == typeof(DBNull))
                return BsonType.Null;
            if(type == typeof(Binary))
                return BsonType.Binary;
            if(type == typeof(Guid))
                return BsonType.Binary;
            if(type == typeof(MongoMinKey))
                return BsonType.MinKey;
            if(type == typeof(MongoMaxKey))
                return BsonType.MaxKey;
            if(type == typeof(MongoSymbol))
                return BsonType.Symbol;
            if(type == typeof(byte[]))
                return BsonType.Binary;
            
            if(_descriptor.IsArray(obj))
                return BsonType.Array;
            if(_descriptor.IsObject(obj))
                return BsonType.Obj;

            if(type == typeof(Decimal))
                throw new ArgumentOutOfRangeException("Decimal is not supported in the BSON spec. So it is also not supported in MongoDB. " +
                                                      "You could convert it to double or store it as Binary instead.");

            throw new ArgumentOutOfRangeException(String.Format("Type: {0} not recognized", type.FullName));
        }
    }
}