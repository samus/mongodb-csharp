using System;

namespace MongoDB
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
    [Serializable]
    public sealed class DBRef : IEquatable<DBRef>
    {
        internal const string IdName = "$id";
        internal const string MetaName = "metadata";
        internal const string RefName = "$ref";

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
            if(document == null)
                throw new ArgumentNullException("document");
            if(IsDocumentDBRef(document) == false)
                throw new ArgumentException("Document is not a valid DBRef");

            _collectionName = (String)document[RefName];
            _id = document[IdName];
            _document = document;
            if(document.ContainsKey("metadata"))
                MetaData = (Document)document["metadata"];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBRef"/> class.
        /// </summary>
        /// <param name="databaseReference">The database reference.</param>
        public DBRef(DBRef databaseReference){
            if(databaseReference == null)
                throw new ArgumentNullException("databaseReference");

            _document = new Document();
            CollectionName = databaseReference.CollectionName;
            Id = databaseReference.Id;
            if(databaseReference.MetaData != null)
                MetaData = new Document().Merge(databaseReference.MetaData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBRef"/> class.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="id">The id.</param>
        public DBRef(string collectionName, object id){
            if(collectionName == null)
                throw new ArgumentNullException("collectionName");
            if(id == null)
                throw new ArgumentNullException("id");

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
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(DBRef) && Equals((DBRef)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){
            unchecked
            {
                var result = (_document != null ? _document.GetHashCode() : 0);
                result = (result*397) ^ (_collectionName != null ? _collectionName.GetHashCode() : 0);
                result = (result*397) ^ (_id != null ? _id.GetHashCode() : 0);
                result = (result*397) ^ (_metadata != null ? _metadata.GetHashCode() : 0);
                return result;
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
            return document != null && document.ContainsKey(RefName) && document.ContainsKey(IdName);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="MongoDB.DBRef"/> to <see cref="MongoDB.Document"/>.
        /// </summary>
        /// <param name="dbRef">The db ref.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Document(DBRef dbRef){
            return dbRef._document;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DBRef other)
        {
            if(ReferenceEquals(null, other))
                return false;
            if(ReferenceEquals(this, other))
                return true;
            return Equals(other._document, _document) && Equals(other._collectionName, _collectionName) && Equals(other._id, _id) && Equals(other._metadata, _metadata);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DBRef left, DBRef right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DBRef left, DBRef right)
        {
            return !Equals(left, right);
        }
    }
}