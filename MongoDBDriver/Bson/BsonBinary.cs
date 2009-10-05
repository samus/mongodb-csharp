using System;
using System.Text;

namespace MongoDB.Driver.Bson
{
	public class BsonBinary:BsonType
    {
        private byte[] val;
        public byte[] Val {
            get {return val;}
            set {val = value;}
        }

        //    Optional data subtype. Default is 0x02 
        //    See http://www.mongodb.org/display/DOCS/BSON#BSON-noteondatabinary
        public enum SubtypeCode : byte{
            Bytes = 2,
            UUID = 3,
            MD5 = 5,
            UserDefined = 8
        }      

        public BsonBinary() { }

        public BsonBinary(byte[] val){
            this.val = val;
            this.subtype = (byte)SubtypeCode.Bytes;            
        }

        public BsonBinary(byte[] val, SubtypeCode subtype)
        {
            this.val = val;
            this.subtype = (byte)subtype;
        }

        public int Size{
            get { return val.Length; }
        }

        public byte TypeNum{
            get { return (byte)BsonDataType.Binary; }
        }

        private byte subtype;
        public byte Subtype {
            get { return this.subtype; }
            set { this.subtype = value; }
        }

        public int Read(BsonReader reader){
            int size = reader.ReadInt32();
            this.Subtype = reader.ReadByte();
            this.Val = reader.ReadBytes(size);
            return this.Size;
        }

        public void Write(BsonWriter writer){
            writer.Write(this.Size);
            writer.Write(this.Subtype);
            writer.Write(this.Val);            
        }

        public virtual object ToNative(){
            return this.Val;
        }

        public override string ToString()
        {
            return string.Format("[BsonBinary: Val={0}, TypeNum={1}, Size={2}, Subtype={3}]", Val, TypeNum, Size, Subtype);
        }

	}
}
