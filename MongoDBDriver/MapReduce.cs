
using System;

namespace MongoDB.Driver
{

    /// <summary>
    /// A fluent interface for executing a Map/Reduce call against a collection.
    /// </summary>
    public class MapReduce
    {
        Database db;
        string name;
        public string Name {
            get {
                return name;
            }
        }      

        CodeWScope mapfunction;        
        public CodeWScope Mapfunction {
            get {
                return mapfunction;
            }
            set {
                mapfunction = value;
            }
        }

        CodeWScope reduceFunction;
        public CodeWScope ReduceFunction {
            get {
                return reduceFunction;
            }
            set {
                reduceFunction = value;
            }
        }

        Document result;
        public Document Result {
            get {
                return result;
            }
            set {
                result = value;
            }
        }
               
        internal MapReduce (Database db, string name){
            this.db = db;
            this.name = name;
        }
        
        #region "Map Functions"
        /// <summary>The map function references the variable this to inspect the current object under consideration.
        /// A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        /// as may be appropriate.
        /// </summary>
        public MapReduce Map(string function){
            return this.Map(new CodeWScope(function, new Document()));
        }
        
        /// <summary>The map function references the variable this to inspect the current object under consideration.
        /// A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        /// as may be appropriate.
        /// </summary>
        public MapReduce Map(string function, Document scope){
            return this.Map(new CodeWScope(function, scope));
        }
        
        /// <summary>The map function references the variable this to inspect the current object under consideration.
        /// A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        /// as may be appropriate.
        /// </summary>
        public MapReduce Map(CodeWScope function){
            mapfunction = function;   
            return this;
        }
        #endregion
        
        #region "Reduce Functions"
        /// <summary>
        /// The reduce function receives a key and an array of values. To use, reduce the received values, 
        /// and return a result.
        /// </summary>
        /// <remarks>The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        /// must be idempotent. If you need to perform an operation only once, use a finalize function.</remarks>
        public MapReduce Reduce(string function){
            return this.Reduce(new CodeWScope(function, new Document()));
        }

        /// <summary>
        /// The reduce function receives a key and an array of values. To use, reduce the received values, 
        /// and return a result.
        /// </summary>
        /// <remarks>The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        /// must be idempotent. If you need to perform an operation only once, use a finalize function.</remarks>
        public MapReduce Reduce(string function, Document scope){
            return this.Reduce(new CodeWScope(function, scope));
        }
        
        /// <summary>
        /// The reduce function receives a key and an array of values. To use, reduce the received values, 
        /// and return a result.
        /// </summary>
        /// <remarks>The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        /// must be idempotent. If you need to perform an operation only once, use a finalize function.</remarks>
        public MapReduce Reduce(CodeWScope function){
            reduceFunction = function;
            return this;
        }
        #endregion
        
        public MapReduce Execute(){
            //need to check state.
            result = db.SendCommand(new Document().Append("mapreduce",this.Name).Append("map", this.mapfunction));
        }
    }
}
