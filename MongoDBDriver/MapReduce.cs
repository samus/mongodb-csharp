
using System;
using System.Collections.Generic;

namespace MongoDB.Driver
{

    /// <summary>
    /// A fluent interface for executing a Map/Reduce call against a collection.
    /// </summary>
    public class MapReduce : IDisposable
    {

        public class MapReduceResult{
            Document result;
            Document counts;
            public MapReduceResult(Document result){
                this.result = result;
                this.counts = (Document)result["counts"];
            }
            public string CollectionName{
                get{return (string)result["result"];}
            }
            public long InputCount{
                get{return Convert.ToInt64(counts["input"]);}
            }
            
            public long EmitCount{
                get{return Convert.ToInt64(counts["emit"]);}
            }
            
            public long OutputCount{
                get{return Convert.ToInt64(counts["output"]);}
            }
            
            public long Time{
                get{return Convert.ToInt64(result["timeMillis"]);}
            }
            
            private TimeSpan span = TimeSpan.MinValue;
            public TimeSpan TimeSpan{
                get{
                    if(span == TimeSpan.MinValue) span = TimeSpan.FromMilliseconds(this.Time);
                    return span;
                }
            }
            
            public Boolean Ok{
                get{return (1.0 == Convert.ToDouble(result["ok"]));}
            }
            
            public String ErrorMessage{
                get{
                    if(result.Contains("msg"))return (String)result["msg"];
                    return String.Empty;
                }
            }
            public override string ToString (){
                return result.ToString();
            }

        }
        
        Database db;
        Document cmd;
        
        #region "Properties"
        string name;
        public string Name {
            get {return (String)cmd["mapreduce"];}
        }      

        MapReduceResult result;
        /// <summary>
        /// Holds the resulting value of the execution.
        /// </summary>
        public MapReduceResult Result {
            get {return result;}
        }
               
        internal MapReduce (Database db, string name){
            this.db = db;
            this.cmd = new Document().Append("mapreduce", name);
            this.Verbose = true;
        }
      
        /// <summary>The map function references the variable this to inspect the current object under consideration.
        /// A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        /// as may be appropriate.
        /// </summary>        
        public Code Map {
            get {return (Code)cmd["map"];}
            set {
                TryModify();
                cmd["map"] = value;}
        }
        
        /// <summary>
        /// The reduce function receives a key and an array of values. To use, reduce the received values, 
        /// and return a result.
        /// </summary>
        /// <remarks>The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        /// must be idempotent. If you need to perform an operation only once, use a finalize function.</remarks>
        public Code Reduce {
            get {return (Code)cmd["reduce"];}
            set {
                TryModify();
                cmd["reduce"] = value;
            }
        }        
        
        #region "Options"
        
        public Document Query{
            get{return (Document)cmd["query"];}
            set{
                TryModify();
                cmd["query"] = value;
            }
        }

        /// <summary>
        /// Sort the query.  Useful for optimization
        /// </summary>
        public Document Sort {
            get {return (Document)cmd["sort"];}
            set {
                TryModify();
                cmd["sort"] = value;
            }
        }

        /// <summary>
        /// Number of objects to return from collection
        /// </summary>
        public long Limit {
            get {return (long)cmd["limit"];}
            set {
                TryModify();
                cmd["limit"] = value;
            }
        }        

        /// <summary>
        /// Name of the final collection the results should be stored in.
        /// </summary>
        /// <remarks>A temporary collection is still used and then renamed to the target name atomically.
        /// </remarks>
        public string Out {
            get {return (string)cmd["out"];}
            set {
                TryModify();
                cmd["out"] = value;
            }
        }        

        /// <summary>
        /// When true the generated collection is not treated as temporary.  Specifying out automatically makes
        /// the collection permanent
        /// </summary>
        public bool KeepTemp {
            get {return Convert.ToBoolean(cmd["keeptemp"]);}
            set {
                TryModify();
                cmd["keeptemp"] = value;
            }
        }

        /// <summary>
        /// Provides statistics on job execution time.
        /// </summary>
        public bool Verbose {
            get {return (bool)cmd["verbose"];}
            set {
                TryModify();
                cmd["verbose"] = value;
            }
        }

        /// <summary>
        /// Function to apply to all the results when finished.
        /// </summary>
        public Code Finalize {
            get {return (Code)cmd["finalize"];}
            set {
                TryModify();
                cmd["finalize"] = value;
            }
        }        

        /// <summary>
        /// Document where fields go into javascript global scope
        /// </summary>
        public Document Scope {
            get {return (Document)cmd["scope"];}
            set {
                TryModify();
                cmd["scope"] = value;
            }
        }
        
        #endregion
        #endregion
        
        public MapReduce Execute(){
            if(cmd.Contains("map") == false || cmd.Contains("reduce") == false){
                throw new InvalidOperationException("Cannot execute without a map and reduce function");
            }
            canModify = false;
            try{
                result = new MapReduce.MapReduceResult(db.SendCommand(cmd));
            }catch(MongoCommandException mce){
                result = new MapReduce.MapReduceResult(mce.Error);
                throw new MongoMapReduceException(mce, this);
            }
            return this;
        }
        
        public IEnumerable<Document> Documents { 
            get {
                if(result == null) Execute(); 
                if(result.Ok == false) 
                    throw new InvalidOperationException("Documents cannot be iterated when an error was returned from execute.");
                
                IEnumerable<Document> docs = db[result.CollectionName].FindAll().Documents;
                using((IDisposable)docs){
                    foreach(Document doc in docs){
                        yield return doc;
                    }
                }
            }
        }
        
        bool canModify = true;
        public Boolean CanModify{
            get{return canModify;}
        }
        
        internal void TryModify(){
            if(canModify == false){
                throw new InvalidOperationException("Cannot modify a map/reduce that has already executed");
            }
        }
        
        #region IDisposable implementation
        private bool disposing = false;
        public void Dispose (){
            if(KeepTemp == true || this.Out != null || disposing == true) return;
            disposing = true;
            
            if(this.result == null || this.result.Ok == false) return; //Nothing to do.
            
            //Drop the temporary collection that was created as part of results.
            db.MetaData.DropCollection(this.result.CollectionName);
        }
        
       #endregion
    }
}
