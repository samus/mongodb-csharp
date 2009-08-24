/*
 * User: scorder
 */
using System;

namespace MongoDB.Driver.Bson
{
	/// <summary>
	/// Description of BsonArray.
	/// </summary>
	public class BsonArray : BsonDocument{
		
		public BsonArray(){}
		
		public override byte TypeNum {
			get {return (byte)BsonDataType.Array;}
		}
		public override BsonElement this[ String key ]  {
			get{
				return (BsonElement)Dictionary[key];
			}
			set{
				int t;
				if(int.TryParse(key,out t) == false)throw new ArgumentOutOfRangeException(String.Format("Key '{0}' isn't numeric", key));
				if(orderedKeys.Contains(key) == false){
					string lastkey = string.Empty;
					if(orderedKeys.Count > 0) lastkey = orderedKeys[orderedKeys.Count -1];
					orderedKeys.Add(key);
					if(lastkey != string.Empty && int.Parse(key) < int.Parse(lastkey)){
						orderedKeys.Sort();
					}
				}
				Dictionary[key] = value;
			}
		}		
		
		public void Add(int key, BsonElement value){
			this.Add(key.ToString(),value);
		}

		public void Add(int key, BsonType val){
			this.Add(new BsonElement(key.ToString(), val));
		}
		
		public void Add(int key, Object val){
			this.Add(new BsonElement(key.ToString(), BsonConvert.From(val)));
		}
		
		public BsonDocument Append(int key, BsonElement value){
			this.Add(key.ToString(),value);
			return this;
		}
		public BsonElement this[ int key ]  {
			get{
				return (BsonElement)Dictionary[key.ToString()];
			}
			set{
				string skey = key.ToString();
				if(orderedKeys.Contains(skey) == false){
					orderedKeys.Add(skey);
				}
				Dictionary[skey] = value;
			}
		}		
		
	}
}
