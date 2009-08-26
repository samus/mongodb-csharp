
using System;

namespace MongoDB.Driver
{
	/// <summary>
	/// Native type that maps to a database reference.
	/// </summary>
	public class DBRef
	{
		private string collectionName;		
		public string CollectionName {
			get { return collectionName; }
			set { collectionName = value; }
		}
		
		private string id;		
		/// <summary>
		/// String value of the id.  It isn't an Oid because document ids are not required to be oids.
		/// </summary>
		public string Id {
			get { return id; }
			set { id = value; }
		}
		
		public DBRef(){}
		
		public DBRef(string collectionName, Oid id){
			this.CollectionName = collectionName;
			this.Id = id.Value;
		}
		
		public DBRef(string collectionName, string id){
			this.CollectionName = collectionName;
			this.Id = id;
		}
		
		public override bool Equals(object obj){
			if(obj.GetType is DBRef){
				DBRef comp = (DBRef)obj;
				return comp.Id.Equals(this.Id) && comp.CollectionName.Equals(this.CollectionName);
			}
			return base.Equals(obj);
		}
		
		public static DBRef FromDocument(Document doc){
			if(IsDocumentDBRef(doc) == false) throw new ArgumentException("Document is not a DBRef");
			DBRef ret = new DBRef();
			ret.CollectionName = (String)doc["$ref"];
			ret.Id = (String)doc["$id"];
			return ret;
		}
		
		public static bool IsDocumentDBRef(Document doc){
			return doc.Contains("$ref") && doc.Contains("$id");
		}
	}
}
