/*
 * User: scorder
 */
using System;
using System.IO;
using System.Text;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
	/// <summary>
	/// Description of BsonOid.
	/// </summary>
	public class BsonOid:BsonType{
		private byte[] val;

		public byte[] Val {
			get {return val;}
			set {val = value;}
		}
		
		public BsonOid(){}
		
		public BsonOid(Oid oid){
			//have to do some conversion here.
			string oidstr = oid.Value;
			if(oidstr.Length % 2 == 1){
				oidstr = "0" + oidstr;
			}
			int numberChars = oidstr.Length;

			byte[] bytes = new byte[numberChars / 2];
			for (int i = 0; i < numberChars; i += 2){
				try{
					bytes[i / 2] = Convert.ToByte(oidstr.Substring(i, 2), 16);
				}
				catch{
					//failed to convert these 2 chars, they may contain illegal charracters
					bytes[i / 2] = 0;
				}
			}
			this.Val = bytes;
				
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
