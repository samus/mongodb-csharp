/*
 * User: scorder
 * Date: 7/15/2009
 */
using System;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Description of BsonInteger.
    /// </summary>
    public class BsonInteger : BsonType
    {
        int val;    
        public int Val {
            get { return val; }
            set { val = value; }
        }
        public int Size {
            get {return 4;}
        }       
        
        public byte TypeNum {
            get {return (byte)BsonDataType.Integer;}
        }

        public BsonInteger(){}
        
        public BsonInteger(int val){
            this.Val = val;
        }
        
        public void Write(BsonWriter writer){
            writer.Write(this.Val);
        }
        
        public int Read(BsonReader reader){
            this.val = reader.ReadInt32();
            return this.Size;
        }
        
        public override string ToString ()
        {
            return string.Format("[BsonInteger: Val={0}, Size={1}, TypeNum={2}]", Val, Size, TypeNum);
        }
        
        public object ToNative(){
            return this.Val;
        }
    }
}
