using System;
using System.Collections.Generic;

namespace MongoDB.Driver{

    /// <summary>
    /// Encapsulates and provides access to the serverside javascript stored in db.system.js.
    /// </summary>
    public class DatabaseJavascript : ICollection<Document>
    {   
        //private Connection connection;
        private MongoDatabase db;
        private IMongoCollection<Document> js;
        
        internal DatabaseJavascript (MongoDatabase db){
            this.db = db;
            this.js = db["system.js"];
            //Needed for some versions of the db to retrieve the functions.
            js.MetaData.CreateIndex(new Document().Add("_id", 1), true);
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
            return js.FindOne(new Document().Add("_id", name));
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
            Add(new Document().Add("_id", name).Add("value", func));
        }
        
        /// <summary>
        /// Store a function in the database with an extended attribute called version.
        /// </summary>
        /// <remarks>Version attributes are an extension to the spec.  Function names must be unique
        /// to the database so only one version can be stored at a time.  This is most useful for libraries
        /// that store function in the database to make sure that the function they are using is the most
        /// up to date.
        /// </remarks>
        public void Add(string name, Code func, float version){
            Add(new Document().Add("_id", name).Add("value", func).Add("version", version));
        }
        
        /// <summary>
        /// Stores a function in the database.
        /// </summary>
        public void Add (Document item){
            if(js.FindOne(new Document().Add("_id", item["_id"])) != null){
                throw new ArgumentException(String.Format("Function {0} already exists in the database.", item["_id"]));
            }
            js.Insert(item);
        }
        
        /// <summary>
        /// Removes every function in the database.
        /// </summary>
        public void Clear (){
            js.Delete(new Document());
        }
        
        public bool Contains (Document item){
            return Contains((string)item["_id"]);
        }
        
        /// <summary>
        /// Checks to see if a function named name is stored in the database.
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public bool Contains (string name){
            return GetFunction(name) != null;
        }
        
        /// <summary>
        /// Copies the functions from the database ordered by _id (name) to the array starting at the index.
        /// </summary>
        /// <param name="array">
        /// A <see cref="Document[]"/> array to coppy to
        /// </param>
        /// <param name="arrayIndex">
        /// A <see cref="System.Int32"/>
        /// </param>
        public void CopyTo (Document[] array, int arrayIndex){
            Document query = new Document().Add("$orderby", new Document().Add("_id", 1));
            int idx = arrayIndex;
            foreach(Document doc in js.Find(query,array.Length - 1,arrayIndex).Documents){
                array[idx] = doc;
                idx++;
            }
        }
        
        public void Update(Document item){
            throw new System.NotImplementedException();
        }
        
        public bool Remove (Document item){
            return Remove((string)item["_id"]);
        }
        
        public bool Remove (string name){
            js.Delete(new Document().Add("_id", name));
            return true;
        }
        
        public int Count {
            get {
                long cnt = js.Count();
                if(cnt > int.MaxValue) return int.MaxValue; //lots of functions.
                return (int)cnt;
            }
        }
        
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        public IEnumerator<Document> GetEnumerator (){
            foreach(Document doc in js.FindAll().Documents){
                yield return doc;
            }
            yield break;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator (){
            return GetEnumerator();
        }
    }
}
