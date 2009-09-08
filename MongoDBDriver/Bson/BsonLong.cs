/*
 * User: scorder
 */
using System;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Description of BsonLong.
    /// </summary>
    public class BsonLong: BsonType
    {
        long val;   
        public long Val {
            get { return val; }
            set { val = value; }
        }
        public int Size {
            get {return 8;}
        }       
        
        public byte TypeNum {
            get {return (byte)BsonDataType.Long;}
        }

        public BsonLong(){}
        
        public BsonLong(long val){
            this.Val = val;
        }
        
        public void Write(BsonWriter writer){
            writer.Write(this.Val);
        }
        
        public int Read(BsonReader reader){
            this.val = reader.ReadInt64();
            return this.Size;
        }
        
        public override string ToString ()
        {
            return string.Format("[BsonLong: Val={0}, Size={1}, TypeNum={2}]", Val, Size, TypeNum);
        }
        
        public object ToNative(){
            return this.Val;
        }       
    }
}
