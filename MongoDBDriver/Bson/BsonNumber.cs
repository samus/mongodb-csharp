/*
 * User: scorder
 * Date: 7/22/2009
 */
using System;

namespace MongoDB.Driver.Bson
{
    /// <summary>
    /// Description of BsonDouble.
    /// </summary>
    public class BsonNumber : BsonType
    {   
        double val; 
        public double Val {
            get { return val; }
            set { val = value; }
        }
        public int Size {
            get {return sizeof(double);}
        }       
        
        public byte TypeNum {
            get {return (byte)BsonDataType.Number;}
        }

        public BsonNumber(){}
        
        public BsonNumber(double val){
            this.Val = val;
        }
        
        public void Write(BsonWriter writer){
            writer.Write(this.Val);
        }
        
        public int Read(BsonReader reader){
            this.val = reader.ReadDouble();
            return this.Size;
        }   
        
        public override string ToString ()
        {
            return string.Format("[BsonNumber: Val={0}, Size={1}, TypeNum={2}]", Val, Size, TypeNum);
        }
        
        public object ToNative(){
            return this.Val;
        }       

    }
}

