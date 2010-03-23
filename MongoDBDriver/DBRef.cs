using System;

namespace MongoDB.Driver
{
    /// <summary>
    ///   Native type that maps to a database reference.  Use Database.FollowReference(DBRef) to retrieve the document
    ///   that it refers to.
    /// </summary>
    /// <remarks>
    ///   DBRefs are just a specification for a specially formatted Document.  At this time the database
    ///   does no special handling of them. Any referential integrity must be maintained by the application
    ///   not the database.
    /// </remarks>
    public class DBRef
    {
        /// <summary>
        /// 
        /// </summary>
        public const string IdName = "$id";

        /// <summary>
        /// 
        /// </summary>
        public const string MetaName = "metadata";

        /// <summary>
        /// 
        /// </summary>
        public const string RefName = "$ref";

        private readonly Document _document;
        private string _collectionName;
        private object _id;
        private Document _metadata;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DBRef" /> class.
        /// </summary>
        public DBRef(){
            _document = new Document();
        }

        /// <summary>
        ///   Constructs a DBRef from a document that matches the DBref specification.
        /// </summary>
        public DBRef(Document document){
            if(IsDocumentDBRef(document) == false)
                throw new ArgumentException("Document is not a valid DBRef");
            _collectionName = (String)document[RefName];
            _id = document[IdName];
            this._document = document;
            if(document.Contains("metadata"))
                MetaData = (Document)document["metadata"];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBRef"/> class.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="id">The id.</param>
        public DBRef(string collectionName, object id){
            _document = new Document();
            CollectionName = collectionName;
            Id = id;
        }

        /// <summary>
        ///   The name of the collection the referenced document is in.
        /// </summary>
        public string CollectionName{
            get { return _collectionName; }
            set{
                _collectionName = value;
                _document[RefName] = value;
            }
        }

        /// <summary>
        ///   Object value of the id.  It isn't an Oid because document ids are not required to be oids.
        /// </summary>
        public object Id{
            get { return _id; }
            set{
                _id = value;
                _document[IdName] = value;
            }
        }

        /// <summary>
        /// An extension to the spec that allows storing of arbitrary data about a reference.
        /// </summary>
        /// <value>The meta data.</value>
        /// <remarks>
        /// This is a non-standard feature.
        /// </remarks>
        public Document MetaData{
            get { return _metadata; }
            set{
                _metadata = value;
                _document[MetaName] = value;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj){
            if(obj is DBRef){
                var comp = (DBRef)obj;
                return comp.Id.Equals(Id) && comp.CollectionName.Equals(CollectionName);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){
            unchecked{
                return ((_collectionName != null ? _collectionName.GetHashCode() : 0)*397) ^ (_id != null ? _id.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString(){
            return _document.ToString();
        }

        /// <summary>
        ///   Deprecated.  Use the new DBRef(Document) constructor instead.
        /// </summary>
        public static DBRef FromDocument(Document document){
            return new DBRef(document);
        }

        /// <summary>
        /// Determines whether [is document DB ref] [the specified document].
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>
        /// 	<c>true</c> if [is document DB ref] [the specified document]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDocumentDBRef(Document document){
            return document != null && document.Contains(RefName) && document.Contains(IdName);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="MongoDB.Driver.DBRef"/> to <see cref="MongoDB.Driver.Document"/>.
        /// </summary>
        /// <param name="dbRef">The db ref.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Document(DBRef dbRef){
            return dbRef._document;
        }
    }
}