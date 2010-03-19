using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Class that knows how to format a native object into bson bits.
    /// </summary>
    public class BsonWriter
    {
        private const int BufferLength = 256;
        private readonly byte[] _buffer;
        private readonly int _maxChars;
        private readonly Stream _stream;
        private readonly IBsonObjectDescriptor _descriptor;
        private readonly BinaryWriter _writer;

        public BsonWriter(Stream stream, IBsonObjectDescriptor descriptor){
            _stream = stream;
            _descriptor = descriptor;
            _writer = new BinaryWriter(_stream);
            _buffer = new byte[BufferLength];
            _maxChars = BufferLength/Encoding.UTF8.GetMaxByteCount(1);
        }
       
        public void WriteValue(BsonDataType dataType, Object obj){
            switch(dataType){
                case BsonDataType.MinKey:
                case BsonDataType.MaxKey:
                case BsonDataType.Null:
                    return;
                case BsonDataType.Boolean:
                    _writer.Write((bool)obj);
                    return;
                case BsonDataType.Integer:
                    _writer.Write((int)obj);
                    return;
                case BsonDataType.Long:
                    _writer.Write((long)obj);
                    return;
                case BsonDataType.Date:
                    Write((DateTime)obj);
                    return;
                case BsonDataType.Oid:
                    Write((Oid)obj);
                    return;
                case BsonDataType.Number:
                    _writer.Write((double)obj);
                    return;
                case BsonDataType.String:{
                    Write((String)obj);
                    return;
                }
                case BsonDataType.Obj:
                    if(obj is DBRef)
                        Write((DBRef)obj);
                    else
                        WriteObject(obj);
                    return;
                case BsonDataType.Array:
                    WriteEnumerable((IEnumerable)obj);
                    return;
                case BsonDataType.Regex:{
                    Write((MongoRegex)obj);
                    return;
                }
                case BsonDataType.Code:{
                    Write((Code)obj);
                    return;
                }
                case BsonDataType.CodeWScope:{
                    Write((CodeWScope)obj);
                    return;
                }
                case BsonDataType.Binary:{
                    if(obj is Guid)
                        Write((Guid)obj);
                    else
                        Write((Binary)obj);
                    return;
                }
                default:
                    throw new NotImplementedException(String.Format("Writing {0} types not implemented.", obj.GetType().Name));
            }
        }
        
        private void Write(Oid id){
            _writer.Write(id.ToByteArray());
        }

        private void Write(Binary binary){
            if(binary.Subtype == Binary.TypeCode.General){
                _writer.Write(binary.Bytes.Length + 4);
                _writer.Write((byte)binary.Subtype);
                _writer.Write(binary.Bytes.Length);
            }
            else{
                _writer.Write(binary.Bytes.Length);
                _writer.Write((byte)binary.Subtype);
            }
            _writer.Write(binary.Bytes);
        }

        private void Write(Guid guid){
            _writer.Write(16);
            _writer.Write((byte)3);
            _writer.Write(guid.ToByteArray());
        }

        private void Write(CodeWScope codeScope){
            _writer.Write(CalculateSize(codeScope));
            WriteValue(BsonDataType.String, codeScope.Value);
            WriteValue(BsonDataType.Obj, codeScope.Scope);
        }

        private void Write(Code code){
            WriteValue(BsonDataType.String, code.Value);
        }

        private void Write(MongoRegex regex){
            Write(regex.Expression,false);
            Write(regex.Options,false);
        }

        public void Write(DBRef reference){
            WriteObject((Document)reference);
        }

        private void Write(DateTime dataTime){
            var diff = dataTime.ToUniversalTime() - BsonInfo.Epoch;
            var time = Math.Floor(diff.TotalMilliseconds);
            _writer.Write((long)time);
        }

        public void WriteObject(object obj){
            obj = _descriptor.BeginObject(obj);
            var propertys = _descriptor.GetPropertyNames(obj);
            var size = CalculateSizeObject(obj,propertys);
            if(size >= BsonInfo.MaxDocumentSize) 
                throw new ArgumentException("Maximum document size exceeded.");
            _writer.Write(size);
            foreach(var name in propertys){
                var value = _descriptor.BeginProperty(obj, name);
                var type = TranslateToBsonType(value);
                _writer.Write((byte)type);
                Write(name, false);
                WriteValue(type, value);
                _descriptor.EndProperty(obj,name,value);
            }
            _writer.Write((byte)0);
            _descriptor.EndObject(obj);
        }

        public void WriteEnumerable(IEnumerable enumerable){
            var size = CalculateSize(enumerable);
            _writer.Write(size);
            var keyname = 0;
            foreach(var val in enumerable){
                var bsonType = TranslateToBsonType(val);
                _writer.Write((byte)bsonType);
                Write(keyname.ToString(),false);
                WriteValue(bsonType, val);
                keyname++;
            }
            _writer.Write((byte)0);
        }

        private void Write(string value)
        {
            Write(value,true);
        }

        public void Write(string value, bool includeLength){
            if(includeLength)
                _writer.Write(CalculateSize(value, false));
            var byteCount = Encoding.UTF8.GetByteCount(value);
            if(byteCount < BufferLength){
                Encoding.UTF8.GetBytes(value, 0, value.Length, _buffer, 0);
                _writer.Write(_buffer, 0, byteCount);
            }
            else{
                int charCount;
                var totalCharsWritten = 0;

                for(var i = value.Length; i > 0; i -= charCount){
                    charCount = (i > _maxChars) ? _maxChars : i;
                    var count = Encoding.UTF8.GetBytes(value, totalCharsWritten, charCount, _buffer, 0);
                    _writer.Write(_buffer, 0, count);
                    totalCharsWritten += charCount;
                }
            }
            _writer.Write((byte)0);
        }
      
        public int CalculateSize(Object obj){
            if(obj == null)
                return 0;

            switch(TranslateToBsonType(obj)){
                case BsonDataType.MinKey:
                case BsonDataType.MaxKey:
                case BsonDataType.Null:
                    return 0;
                case BsonDataType.Boolean:
                    return 1;
                case BsonDataType.Integer:
                    return 4;
                case BsonDataType.Long:
                case BsonDataType.Date:
                    return 8;
                case BsonDataType.Oid:
                    return 12;
                case BsonDataType.Number:
                    return sizeof(Double);
                case BsonDataType.String:
                    return CalculateSize((string)obj);
                case BsonDataType.Obj:{
                    if(obj.GetType() == typeof(DBRef))
                        return CalculateSize((DBRef)obj);
                    return CalculateSizeObject(obj);
                }
                case BsonDataType.Array:
                    return CalculateSize((IEnumerable)obj);
                case BsonDataType.Regex:{
                    return CalculateSize((MongoRegex)obj);
                }
                case BsonDataType.Code:
                    return CalculateSize((Code)obj);
                case BsonDataType.CodeWScope:{
                    return CalculateSize((CodeWScope)obj);
                }
                case BsonDataType.Binary:{
                    if(obj is Guid)
                        return CalculateSize((Guid)obj);
                    return CalculateSize((Binary)obj);
                }
            }

            throw new NotImplementedException(String.Format("Calculating size of {0} is not implemented.", obj.GetType().Name));
        }
        
        private int CalculateSize(Code code){
            return CalculateSize(code.Value, true);
        }

        public int CalculateSize(MongoRegex regex){
            var size = CalculateSize(regex.Expression, false);
            size += CalculateSize(regex.Options, false);
            return size;
        }

        public int CalculateSize(CodeWScope codeScope){
            var size = 4;
            size += CalculateSize(codeScope.Value, true);
            size += CalculateSizeObject(codeScope.Scope);
            return size;
        }

        public int CalculateSize(Binary binary){
            var size = 4; //size int
            size += 1; //subtype
            if(binary.Subtype == Binary.TypeCode.General)
                size += 4; //embedded size int
            size += binary.Bytes.Length;
            return size;
        }

        public int CalculateSize(Guid guid){
            return 21;
        }

        public int CalculateSize(DBRef reference){
            return CalculateSizeObject((Document)reference);
        }
        

        public int CalculateSizeObject(object obj){
            obj = _descriptor.BeginObject(obj);
            var propertys = _descriptor.GetPropertyNames(obj);

            var size = CalculateSizeObject(obj, propertys);

            _descriptor.EndObject(obj);

            return size;
        }

        private int CalculateSizeObject(object obj, IEnumerable<string> propertys)
        {
            var size = 4;
            foreach(var name in propertys)
            {
                var elsize = 1; //type
                var value = _descriptor.BeginProperty(obj, name);
                elsize += CalculateSize(name, false);
                _descriptor.EndProperty(obj, name, value);
                elsize += CalculateSize(value);
                size += elsize;
            }
            size += 1; //terminator
            return size;
        }
        
        public int CalculateSize(IEnumerable enumerable){
            var size = 4; //base size for the object
            var keyname = 0;
            foreach(var o in enumerable){
                size += CalculateSize(keyname.ToString(), false); //element name
                size += CalculateSize(o);
                size += 1; // elsize
                keyname++;
            }
            size += 1; //terminator
            return size;
        }
        
        public int CalculateSize(String value){
            return CalculateSize(value, true);
        }

        public int CalculateSize(String value, bool includeLength){
            var size = 1; //terminator
            if(includeLength)
                size += 4;
            if(value != null)
                size += Encoding.UTF8.GetByteCount(value);
            return size;
        }

        public void Flush(){
            _writer.Flush();
        }
        
        protected BsonDataType TranslateToBsonType(object obj){
            if(obj == null)
                return BsonDataType.Null;

            var type = obj.GetType();

            if(obj is Enum) //special case enums               
                type = Enum.GetUnderlyingType(type);

            if(type == typeof(Double))
                return BsonDataType.Number;
            if(type == typeof(Single))
                return BsonDataType.Number;
            if(type == typeof(String))
                return BsonDataType.String;
            if(type == typeof(int))
                return BsonDataType.Integer;
            if(type == typeof(long))
                return BsonDataType.Long;
            if(type == typeof(bool))
                return BsonDataType.Boolean;
            if(type == typeof(Oid))
                return BsonDataType.Oid;
            if(type == typeof(DateTime))
                return BsonDataType.Date;
            if(type == typeof(MongoRegex))
                return BsonDataType.Regex;
            if(type == typeof(DBRef))
                return BsonDataType.Obj;
            if(type == typeof(Code))
                return BsonDataType.Code;
            if(type == typeof(CodeWScope))
                return BsonDataType.CodeWScope;
            if(type == typeof(DBNull))
                return BsonDataType.Null;
            if(type == typeof(Binary))
                return BsonDataType.Binary;
            if(type == typeof(Guid))
                return BsonDataType.Binary;
            if(type == typeof(MongoMinKey))
                return BsonDataType.MinKey;
            if(type == typeof(MongoMaxKey))
                return BsonDataType.MaxKey;

            if(_descriptor.IsArray(obj))
                return BsonDataType.Array;
            if(_descriptor.IsObject(obj))
                return BsonDataType.Obj;

            throw new ArgumentOutOfRangeException(String.Format("Type: {0} not recognized", type.FullName));
        }
    }
}