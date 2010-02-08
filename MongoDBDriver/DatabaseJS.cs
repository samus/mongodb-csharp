using System;
using System.Collections.Generic;

namespace MongoDB.Driver{

    /// <summary>
    /// Encapsulates and provides access to the serverside javascript stored in db.system.js.
    /// </summary>
    public class DatabaseJS : ICollection<Document>
    {   
        //private Connection connection;
        private Database db;
        private IMongoCollection js;
        
        internal DatabaseJS (Database db){
            this.db = db;
            this.js = db["system.js"];
            //Needed for some versions of the db to retrieve the functions.
            js.MetaData.CreateIndex(new Document().Append("_id",1),true);
        }
        
        public Document this[ String name ]  {
            get{
                return GetFunction(name);
            }
            set{
                Add(value);
            }            
        }

        /// <summary>
        /// Gets the document representing the function in the database.
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="Document"/>
        /// </returns>
        public Document GetFunction(string name){
            return js.FindOne(new Document().Append("_id", name));
        }
        
        /// <summary>
        /// Returns a listing of the names of all the functions in the database
        /// </summary>
        public List<string> GetFunctionNames(){
            List<string> l = new List<string>();
            foreach(Document d in js.FindAll().Documents){
                l.Add((String)d["_id"]);
            }
            return l;
        }
        
        public void Add (string name, string func){
            Add(name, new Code(func));            
        }

        public void Add (string name, Code func){
            Add(new Document().Append("_id", name).Append("value", func));
        }
        
        /// <summary>
        /// Stores a function in the database.
        /// </summary>
        public void Add (Document item){
            if(js.FindOne(new Document().Append("_id", item["_id"])) != null){
                throw new ArgumentException(String.Format("Function {0} already exists in the database.", item["_id"]));
            }
            js.Insert(item);
        }
        
        public void Clear (){
            throw new System.NotImplementedException();
        }
        
        public bool Contains (Document item){
            return Contains((string)item["_id"]);
        }
        
        public bool Contains (string name){
            return GetFunction(name) != null;
        }
        
        
        public void CopyTo (Document[] array, int arrayIndex){
            throw new System.NotImplementedException();
        }
        
        
        public bool Remove (Document item){
            throw new System.NotImplementedException();
        }
        
        
        public int Count {
            get {
                throw new System.NotImplementedException();
            }
        }
        
        
        public bool IsReadOnly {
            get {
                throw new System.NotImplementedException();
            }
        }
        
        #region IEnumerable<Document> implementation
        public IEnumerator<Document> GetEnumerator (){
            throw new System.NotImplementedException();
        }
        
        #endregion
        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator (){
            throw new System.NotImplementedException();
        }
        
        #endregion
        
    }
}
