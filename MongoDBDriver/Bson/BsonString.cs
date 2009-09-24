
using System;
using System.Text;

namespace MongoDB.Driver.Bson
{
    public class BsonString:BsonType
    {
        private UTF8Encoding encoding = new UTF8Encoding();
        private String val;
        public string Val {
            get {return val;}
            set {val = value;}
        }
        
        public virtual byte TypeNum {
            get{
                return (byte)BsonDataType.String;
            }
        }
        
        public int Size {
            get {
                int basesize = 5; //size bytes + terminator            
                int ret;
                if(this.val != null) {
                    ret = encoding.GetByteCount(this.val);
                }else{
                    ret = 0;
                }
                return basesize + ret;
            }
        }


        public BsonString(){}
        
        public BsonString(String str){
            this.Val = str; 
        }
        
        public void Write(BsonWriter writer){
            //TODO Calling GetByteCount may be expensive.  Possibly just do the same work write(String) does.
            writer.Write(encoding.GetByteCount(this.val) + 1);
            writer.Write(this.Val);
        }
        
        public int Read(BsonReader reader){
            int len = reader.ReadInt32();
            this.Val = reader.ReadString(len);
            return 4 + len;
        }
        
        public override string ToString (){
            return string.Format("[BsonString: Val={0}, TypeNum={1}, Size={2}]", Val, TypeNum, Size);
        }
        
        public virtual object ToNative(){
            return this.Val;
        }
    }
}
