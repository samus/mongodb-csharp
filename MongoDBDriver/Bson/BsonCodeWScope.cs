using System;

using MongoDB;

namespace MongoDB.Driver.Bson
{
    public class BsonCodeWScope:BsonType{
        private string val;
        public string Val {
            get {return val;}
            set {val = value;}
        }
        
        private BsonDocument scope;
        public BsonDocument Scope {
            get {return scope;}
            set {
                scope = value;
            }
        }
        
        public int Size {
            get {
                int ret = 8;//total size + cstr size
                if(this.val != null) {
                    ret += this.val.Length;
                }else{
                    ret += 0;
                }
                ret += 1; //terminator
                ret += scope.Size;
                return ret;
            }
        }
        
        public byte TypeNum {
            get {return (byte)BsonDataType.CodeWScope;}
        }
        
        public BsonCodeWScope(){}
        
        public BsonCodeWScope(CodeWScope code){
            this.Val = code.Value;
            this.Scope = BsonConvert.From(code.Scope);
        }
        
        public int Read(BsonReader reader){
            int size = reader.ReadInt32();
            int bytesRead = 4;
            
            int codeLen = reader.ReadInt32();
            this.Val = reader.ReadString(codeLen);
            bytesRead += 4 + codeLen;
            
            this.Scope = new BsonDocument();
            bytesRead += this.Scope.Read(reader);
            
            if(bytesRead != size){
                throw new System.IO.InvalidDataException(string.Format("Should have read {0} bytes from stream but only read {1}]", size, bytesRead));
            }
            
            return bytesRead;
        }
        
        public void Write(BsonWriter writer){
            writer.Write(this.Size); //object size
            writer.Write(this.Val.Length + 1); //code size
            writer.Write(this.Val);
            this.Scope.Write(writer);
        }
        
        public object ToNative(){
            CodeWScope ret = new CodeWScope();
            ret.Scope = (Document)this.Scope.ToNative();
            ret.Value = this.Val;
            return ret;
        }
    }

    public class BsonCode : BsonString
    {
        public override byte TypeNum {
            get {return (byte)BsonDataType.Code;}
        }
        
        public BsonCode(){
        }
        
        public BsonCode(Code val){
            this.Val = val.Value;
        }
        
        public override object ToNative (){
            return new Code(Val);
        }
        
        public override string ToString (){
            return string.Format("[BsonCode: Val={0}, TypeNum={1}, Size={2}]", Val, TypeNum, Size);
        }
    }
}
