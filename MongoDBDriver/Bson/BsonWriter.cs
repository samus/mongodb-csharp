using System;
using System.Collections;
using System.IO;
using System.Text;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Class that knows how to format a native object into bson bits.
    /// </summary>
    public class BsonWriter
    {
        private Stream stream;
        private BinaryWriter writer;
        private Encoding encoding = Encoding.UTF8;
        private int buffLength = 256;
        private byte[] buffer;
        int maxChars;
        
        public BsonWriter(Stream stream){
            this.stream = stream;
            writer = new BinaryWriter(this.stream);
            buffer = new byte[buffLength];
            maxChars = buffLength / encoding.GetMaxByteCount(1);
        }
        
        public void Write(Document doc){
            int size = CalculateSize(doc);
            writer.Write(size);
            foreach(String key in doc.Keys){
                Object val = doc[key];
                BsonDataType t = TranslateToBsonType(val);
                writer.Write((byte)t);
                this.WriteString(key);
                this.WriteValue(t,val);
            }
            writer.Write((byte)0);
        }
        
        public void WriteArray(IEnumerable arr){
            int size = CalculateSize(arr);
            writer.Write(size);
            int keyname = 0;
            foreach(Object val in arr){
                BsonDataType t = TranslateToBsonType(val);
                writer.Write((byte)t);
                this.WriteString(keyname.ToString());
                this.WriteValue(t,val);
                keyname++;
            }
            writer.Write((byte)0);
        }        
        
        public void WriteValue(BsonDataType dt, Object obj){
            switch (dt){
                case BsonDataType.MinKey:
                case BsonDataType.MaxKey:
                case BsonDataType.Null:
                    return;
                case BsonDataType.Boolean:
                    writer.Write((bool)obj);
                    return;
                case BsonDataType.Integer:
                    writer.Write((int)obj);
                    return;
                case BsonDataType.Long:
                    writer.Write((long)obj);
                    return;                    
                case BsonDataType.Date:
                    DateTime d = (DateTime)obj;
                    TimeSpan diff = d.ToUniversalTime() - BsonInfo.Epoch;
                    double time = Math.Floor(diff.TotalMilliseconds);
                    writer.Write((long)time);
                    return;
                case BsonDataType.Oid:
                    Oid id = (Oid) obj;
                    writer.Write(id.Value);
                    return;
                case BsonDataType.Number:
                    writer.Write((double)obj);
                    return;
                case BsonDataType.String:{
                    String str = (String)obj;
                    writer.Write(CalculateSize(str,false));
                    this.WriteString(str);
                    return;
                    }
                case BsonDataType.Obj:
                    if(obj is Document){
                        this.Write((Document)obj);
                    }else if(obj is DBRef){
                        this.Write((Document)((DBRef)obj));
                    }
                    return;
                case BsonDataType.Array:
                    this.WriteArray((IEnumerable)obj);
                    return;
                case BsonDataType.Regex:{
                    MongoRegex r = (MongoRegex)obj;
                    this.WriteString(r.Expression);
                    this.WriteString(r.Options);
                    return;
                }
                case BsonDataType.Code:{
                    Code c = (Code)obj;
                    this.WriteValue(BsonDataType.String,c.Value);
                    return;
                }
                case BsonDataType.CodeWScope:{
                    CodeWScope cw = (CodeWScope)obj;
                    writer.Write(CalculateSize(cw));
                    this.WriteValue(BsonDataType.String,cw.Value);
                    this.WriteValue(BsonDataType.Obj,cw.Scope);
                    return;
                }
                case BsonDataType.Binary:{
                    if (obj is Guid) {
                        writer.Write((int)16);
                        writer.Write((byte)3);
                        writer.Write(((Guid)obj).ToByteArray());
                    } else {
                        Binary b = (Binary)obj;
                        if(b.Subtype == Binary.TypeCode.General){
                            writer.Write(b.Bytes.Length + 4);
                            writer.Write((byte)b.Subtype);
                            writer.Write(b.Bytes.Length);
                        }else{
                            writer.Write(b.Bytes.Length);
                            writer.Write((byte)b.Subtype);
                        }
                        writer.Write(b.Bytes);
                    }
                    return;
                }
                default:
                    throw new NotImplementedException(String.Format("Writing {0} types not implemented.",obj.GetType().Name));                
            }
        }
        
        public void WriteString(String str){
            int byteCount = encoding.GetByteCount(str);
            if(byteCount < buffLength){
                encoding.GetBytes(str,0,str.Length,buffer,0);
                writer.Write(buffer,0,byteCount);
            }else{
                int charCount;
                int totalCharsWritten = 0;
                
                for (int i = str.Length; i > 0; i -= charCount){
                  charCount = (i > maxChars) ? maxChars : i;
                  int count = encoding.GetBytes(str, totalCharsWritten, charCount, buffer, 0);
                  writer.Write(buffer, 0, count);
                  totalCharsWritten += charCount;
                }
            }
            writer.Write((byte)0);
        }
        
        public int CalculateSize(Object val){
            if(val == null) return 0;
            switch (TranslateToBsonType(val)){
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
                    return CalculateSize((string)val);
                case BsonDataType.Obj:{
                    Type t = val.GetType();
                    if(t == typeof(Document)){
                        return CalculateSize((Document)val);
                    }
                    if(t == typeof(DBRef)){
                        return CalculateSize((Document)((DBRef)val));
                    }
                    throw new NotImplementedException(String.Format("Calculating size of {0} is not implemented yet.",t.Name));
                }
                case BsonDataType.Array:
                    return CalculateSize((IEnumerable)val);                    
                case BsonDataType.Regex:{
                    MongoRegex r = (MongoRegex)val;
                    int size = CalculateSize(r.Expression,false);
                    size += CalculateSize(r.Options,false);
                    return size;
                    }
                case BsonDataType.Code:
                    Code c = (Code)val;
                    return CalculateSize(c.Value,true);
                case BsonDataType.CodeWScope:{
                    CodeWScope cw = (CodeWScope)val;
                    int size = 4;
                    size += CalculateSize(cw.Value,true);
                    size += CalculateSize(cw.Scope);
                    return size;
                    }
                case BsonDataType.Binary:{
                    if (val is Guid)
                        return 21;
                    Binary b = (Binary)val;
                    int size = 4; //size int
                    size += 1; //subtype
                    if (b.Subtype == Binary.TypeCode.General)
                    {
                        size += 4; //embedded size int
                    }
                    size += b.Bytes.Length;
                    return size;
                }
                default:
                    throw new NotImplementedException(String.Format("Calculating size of {0} is not implemented.",val.GetType().Name));
            }
        }
        
        public int CalculateSize(Document doc){
            int size = 4;
            foreach(String key in doc.Keys){
                int elsize = 1; //type
                elsize += CalculateSize(key,false);
                elsize += CalculateSize(doc[key]);
                size += elsize;
            }            
            size += 1; //terminator
            return size;
        }
        
        public int CalculateSize(IEnumerable arr){
            int size = 4;//base size for the object
            int keyname = 0;
            foreach(Object o in arr){
                int elsize = 1; //type
                size += CalculateSize(keyname.ToString(),false); //element name
                size += CalculateSize(o);
                size += elsize;
                keyname++;    
            }            
            size += 1; //terminator
            return size;
        }
        
        public int CalculateSize(String val){
            return CalculateSize(val, true);
        }
        
        public int CalculateSize(String val, bool includeLen){
            int size = 1; //terminator
            if(includeLen) size += 4;
            if(val != null) size += encoding.GetByteCount(val);
            return size;
        }        
        
        public void Flush(){
            writer.Flush();
        }
        
        protected BsonDataType TranslateToBsonType(Object val){
            if(val == null)return BsonDataType.Null;
            Type t = val.GetType();
            //special case enums
            if(val is Enum){
                t = Enum.GetUnderlyingType(t);
            }        
            BsonDataType ret;
            if(t == typeof(Double)){
                ret = BsonDataType.Number;
            }else if(t == typeof(Single)){
                ret = BsonDataType.Number;
            }else if(t == typeof(String)){
                ret = BsonDataType.String;
            }else if(t == typeof(Document)){
                ret = BsonDataType.Obj;
            }else if(t == typeof(int)){
                ret = BsonDataType.Integer;
            }else if(t == typeof(long)){
                ret = BsonDataType.Long;
            }else if(t == typeof(bool)){
                ret = BsonDataType.Boolean;
            }else if(t == typeof(Oid)){
                ret = BsonDataType.Oid;
            }else if(t == typeof(DateTime)){
                ret = BsonDataType.Date;
            }else if(t == typeof(MongoRegex)){
                ret = BsonDataType.Regex;
            }else if(t == typeof(DBRef)){
                ret = BsonDataType.Obj;
            }else if(t == typeof(Code)){
                ret = BsonDataType.Code;
            }else if(t == typeof(CodeWScope)){
                ret = BsonDataType.CodeWScope;
            }else if(t == typeof(DBNull)){ 
                ret = BsonDataType.Null;
            }else if(t == typeof(Binary)){
                ret = BsonDataType.Binary;
            }else if(t == typeof(Guid)){
                ret = BsonDataType.Binary;
            }else if(t == typeof(MongoMinKey)){
                ret = BsonDataType.MinKey;
            }else if(t == typeof(MongoMaxKey)){
                ret = BsonDataType.MaxKey;
            }else if(val is IEnumerable){
                ret = BsonDataType.Array;
            }else{
                throw new ArgumentOutOfRangeException(String.Format("Type: {0} not recognized",t.FullName));
            }
            return ret;
        }
    }
}
