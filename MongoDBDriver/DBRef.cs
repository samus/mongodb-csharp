using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Native type that maps to a database reference.  Use Database.FollowReference(DBRef) to retrieve the document
    /// that it refers to.
    /// </summary>
    /// <remarks>DBRefs are just a specification for a specially formatted Document.  At this time the database
    /// does no special handling of them. Any referential integrity must be maintained by the application 
    /// not the database.
    /// </remarks>
    public class DBRef
    {
        public const string RefName = "$ref";
        public const string IdName = "$id";
        public const string MetaName = "metadata";
        
        private Document document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBRef"/> class.
        /// </summary>
        public DBRef(){
            document = new Document();
        }

        private string collectionName;
        /// <summary>
        /// The name of the collection the referenced document is in. 
        /// </summary>
        public string CollectionName {
            get { return collectionName; }
            set {
                collectionName = value; 
                document[RefName] = value;
            }
        }

        private object id;
        /// <summary>
        /// Object value of the id.  It isn't an Oid because document ids are not required to be oids.
        /// </summary>
        public object Id {
            get { return id; }
            set {
                id = value; 
                document[IdName] = value;
            }
        }

        private Document metadata;
        /// <summary>
        /// An extension to the spec that allows storing of arbitrary data about a reference.  
        /// </summary>
        /// <remarks>This is a non-standard feature.
        /// </remarks>
        public Document MetaData {
            get{return metadata; }
            set{
                metadata = value;
                document[MetaName] = value;
            }
        }

        /// <summary>
        /// Constructs a DBRef from a document that matches the DBref specification.
        /// </summary>
        public DBRef(Document document){
            if(IsDocumentDBRef(document) == false) throw new ArgumentException("Document is not a valid DBRef");
            collectionName = (String)document[RefName];
            id = document[IdName];
            this.document = document;
            if(document.Contains("metadata")) this.MetaData = (Document)document["metadata"];
        }
        
        public DBRef(string collectionName, object id){
            document = new Document();
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

        public override int GetHashCode(){
            unchecked{
                return ((this.collectionName != null ? this.collectionName.GetHashCode() : 0) * 397) ^ (this.id != null ? this.id.GetHashCode() : 0);
            }
        }
        
        public override string ToString (){
            return document.ToString();
        }


        /// <summary>
        /// Deprecated.  Use the new DBRef(Document) constructor instead.
        /// </summary>
        public static DBRef FromDocument(Document doc){
            return new DBRef(doc);;
        }
        
        public static bool IsDocumentDBRef(Document doc){
            return doc != null && doc.Contains(RefName) && doc.Contains(IdName);
        }
        
        public static explicit operator Document(DBRef d){
            return d.document;
        }        
    }
}
