
using System;

namespace MongoDB.Driver.Bson
{
    
    
    public class BsonElement
    {
        private string name;
        public string Name {
            get {return name;}
            set {name = value;}
        }
        
        private BsonType val;
        public BsonType Val {
            get {return val;}
            set {val = value;}
        }
        
        public BsonElement()
        {}
        
        public BsonElement(String name, BsonType val){
            this.Name = name;
            this.Val = val;
        }

        public int Size{
            get{
                Int32 ret = 1; //TypeNum
                ret += this.Name.Length + 1; //Name
                ret += this.Val.Size; //Object data
                return ret;
            }
        }
        
        public void Write(BsonWriter writer){
            writer.Write(this.Val.TypeNum); 
            writer.Write(this.Name);
            this.Val.Write(writer);
        }
                
        public int Read(BsonReader reader){
            sbyte typeNum = (sbyte)reader.ReadByte();
            int bytesRead;
            this.Name = reader.ReadString();
            this.Val = BsonConvert.Create((BsonDataType)typeNum);
            bytesRead = this.Val.Read(reader);
            bytesRead += (1 + this.Name.Length + 1); //type byte & name + term
            return bytesRead;
        }
    }
}
