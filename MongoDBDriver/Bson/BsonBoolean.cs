/*
 * User: scorder
 * Date: 7/15/2009
 */
using System;

namespace MongoDB.Driver.Bson
{
	/// <summary>
	/// Description of BsonBoolean.
	/// </summary>
	public class BsonBoolean : BsonType
	{
		bool val;	
		public bool Val {
			get { return val; }
			set { val = value; }
		}
		
		public BsonBoolean(){}
		
		public BsonBoolean(bool val){
			this.Val = val;
		}
		
		public byte TypeNum {
			get {return (byte)BsonDataType.Boolean;}
		}
		
		public int Size{
			get {return 1;}
		}
		
		public void Write(BsonWriter writer){
			writer.Write(this.Val);
		}		
		
		public int Read(BsonReader reader){
			this.val = reader.ReadBoolean();
			return 1;
		}
		
		public override string ToString ()
		{
			return string.Format("[BsonBoolean: Val={0}, TypeNum={1}, Size={2}]", Val, TypeNum, Size);
		}		

		public object ToNative(){
			return this.Val;
		}
	}
}
