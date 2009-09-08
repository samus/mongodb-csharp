/*
 * User: scorder
 * Date: 7/15/2009
 */
using System;

using MongoDB.Driver;

namespace MongoDB.Driver.Bson
{
	/// <summary>
	/// Description of BsonConvert.
	/// </summary>
	public static class BsonConvert
	{
	
		public static BsonType From(Object val){
			Type t = val.GetType();
			//special case enums
			if(val is Enum){
				t = Enum.GetUnderlyingType(t);
			}
			
			BsonType ret = null;
			if(t == typeof(Double)){
				ret = From((double)val);
			}else if(t == typeof(Single)){
				ret = From((double)val);
			}else if(t == typeof(String)){
				ret = From((String)val);
			}else if(t == typeof(Document)){
				ret = From((Document)val);
			}else if(t == typeof(int)){
				ret = From((int)val);
			}else if(t == typeof(long)){
				ret = From((long)val);
			}else if(t == typeof(bool)){
				ret = From((bool)val);
			}else if(t == typeof(Oid)){
				ret = From((Oid)val);
			}else if(t == typeof(DateTime)){
				ret = From((DateTime)val);
			}else if(t == typeof(MongoRegex)){
				ret = From((MongoRegex)val);
			}else if(t == typeof(DBRef)){
				ret = From((DBRef)val);
			}else{
				throw new ArgumentOutOfRangeException(String.Format("Type: {0} not recognized",t.FullName));
			}
			
			return ret;
		}
		
		public static BsonDocument From(Document doc){
			BsonDocument bdoc = new BsonDocument();
			foreach(String key in doc.Keys){
				bdoc.Add(key, From(doc[key]));
			}
			return bdoc;
		}
		
		public static BsonOid From(Oid val){
			return new BsonOid(val);
		}
		
		public static BsonInteger From(int val){
			return new BsonInteger(val);
		}
		
		public static BsonLong From(long val){
			return new BsonLong(val);
		}
		
		public static BsonNumber From(double val){
			return new BsonNumber(val);
		}
		
		public static BsonString From(string val){
			return new BsonString(val);
		}
	
		public static BsonBoolean From(bool val){
			return new BsonBoolean(val);
		}

		public static BsonDate From(DateTime val){
			return new BsonDate(val);
		}		
		
		public static BsonRegex From(MongoRegex val){
			return new BsonRegex(val.Expression, val.Options);
		}
		
		public static BsonDocument From(DBRef val){
			BsonDocument ret = new BsonDocument();
			ret.Add(new BsonElement("$ref", new BsonString(val.CollectionName)));
			ret.Add(new BsonElement("$id", From(val.Id)));
			return ret;
		}
		
		public static BsonType Create(BsonDataType type){
			BsonType ret = null;
			if(type == BsonDataType.Number){
				ret = new BsonNumber();
			}else if(type == BsonDataType.Number){
				throw new NotImplementedException();				
			}else if(type == BsonDataType.String){
				ret = new BsonString();
			}else if(type == BsonDataType.Obj){
				ret = new BsonDocument();
			}else if(type == BsonDataType.Array){
				ret = new BsonArray();				
			}else if(type == BsonDataType.Integer){
				ret = new BsonInteger();
			}else if(type == BsonDataType.Long){
				ret = new BsonLong();
			}else if(type == BsonDataType.Boolean){
				ret = new BsonBoolean();
			}else if(type == BsonDataType.Oid){
				ret = new BsonOid();
			}else if(type == BsonDataType.Date){
				ret = new BsonDate();
			}else if(type == BsonDataType.Regex){
				ret = new BsonRegex();
			}else if(type == BsonDataType.Undefined){
				ret = new BsonUndefined();
			}else if(type == BsonDataType.Null){
				ret = new BsonNull();
			}else{
				string typename = Enum.GetName(typeof(BsonDataType), type);
				throw new ArgumentOutOfRangeException("type",typename + "is not recognized");
			}			
			return ret;			
		}
	}
}
