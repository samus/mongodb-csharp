
using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Native type that maps to a database reference.
    /// </summary>
    public class DBRef
    {
        public const string RefName = "$ref";
        public const string IdName = "$id";
        
        private string collectionName;      
        public string CollectionName {
            get { return collectionName; }
            set { collectionName = value; }
        }
        
        private object id;      
        /// <summary>
        /// Object value of the id.  It isn't an Oid because document ids are not required to be oids.
        /// </summary>
        public object Id {
            get { return id; }
            set { id = value; }
        }
        
        public DBRef(){}

        public DBRef(string collectionName, object id){
            this.CollectionName = collectionName;
            this.Id = id;
        }       

        public override bool Equals(object obj){
            if(obj is DBRef){
                DBRef comp = (DBRef)obj;
                return comp.Id.Equals(this.Id) && comp.CollectionName.Equals(this.CollectionName);
            }
            return base.Equals(obj);
        }

        public static DBRef FromDocument(Document doc){
            if(IsDocumentDBRef(doc) == false) throw new ArgumentException("Document is not a DBRef");
            DBRef ret = new DBRef();
            ret.CollectionName = (String)doc[DBRef.RefName];
            ret.Id = doc[DBRef.IdName];
            return ret;
        }
        
        public static bool IsDocumentDBRef(Document doc){
            return doc != null && doc.Contains(DBRef.RefName) && doc.Contains(DBRef.IdName);
        }
    }
}
