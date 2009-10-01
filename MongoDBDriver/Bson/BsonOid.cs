/*
 * User: scorder
 */
using System;
using System.IO;
using System.Text;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
    public class BsonOid:BsonType{
        private byte[] val;

        public byte[] Val {
            get {return val;}
            set {val = value;}
        }
        
        public BsonOid(){}
        
        public BsonOid(Oid oid){
            this.val = oid.Value;    
        }
        
        public int Size {
            get {return val.Length;}
        }
        
        public byte TypeNum {
            get {return (byte)BsonDataType.Oid;}
        }
        
        public int Read(BsonReader reader){
            this.Val = reader.ReadBytes(12);
            return this.Size;
        }
        
        public void Write(BsonWriter writer){
            writer.Write(this.val);
        }
        
        public object ToNative(){
            string val = BitConverter.ToString(this.Val).Replace("-","");
            return new Oid(val);
        }
    }
}
