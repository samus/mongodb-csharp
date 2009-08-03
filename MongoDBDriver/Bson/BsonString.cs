
using System;

namespace MongoDB.Driver.Bson
{
	public class BsonString:BsonType
	{
		private String val;

		public string Val {
			get {return val;}
			set {val = value;}
		}

		public BsonString(){}
		
		public BsonString(String str){
			this.Val = str;	
		}
		
		public byte TypeNum {
			get{
				return (byte)BsonDataType.String;
			}
		}
		
		public int Size {
			get {
				int ret = 0;
				ret = 4; //size bytes
				if(this.val != null) {
					ret += this.val.Length;
				}else{
					ret += 0;
				}
				ret += 1; //terminator
				return ret;
			}
		}
		
		public void Write(BsonWriter writer){
			writer.Write(this.Val.Length + 1);
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
		
		public object ToNative(){
			return this.Val;
		}		
	}
}
