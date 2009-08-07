/*
 * User: scorder
 */
using System;

namespace MongoDB.Driver.Bson
{
	/// <summary>
	/// Description of BsonDate.
	/// </summary>
	public class BsonDate:BsonType
	{
		private static DateTime epoch = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
		
		private long val;

		public long Val {
			get {return val;}
			set {val = value;}
		}

		public byte TypeNum {
			get{
				return (byte)BsonDataType.Date;
			}
		}
		
		public int Size {
			get {
				return sizeof(long);
			}
		}		
		
		public BsonDate(){}
		
		public BsonDate(long val){
			this.Val = val;	
		}
		
		public BsonDate(DateTime val){
			TimeSpan diff = val.ToUniversalTime() - epoch;
			double time = Math.Floor(diff.TotalMilliseconds);
			this.Val = (long)time;
		}
		
		public void Write(BsonWriter writer){
			writer.Write(this.Val);
		}
		
		public int Read(BsonReader reader){
			this.Val = reader.ReadInt64();
			return this.Size;
		}
		
		public override string ToString (){
			return string.Format("[BsonDate: Val={0}, TypeNum={1}, Size={2}]", Val, TypeNum, Size);
		}
		
		public object ToNative(){
			return epoch.AddMilliseconds(this.Val);
		}		
	}
}
