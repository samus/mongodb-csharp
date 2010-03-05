using System;

namespace MongoDB.Driver
{
    /// <summary>
    /// Provides a Fluent interface to build and execute Map/Reduce calls.
    /// </summary>
    public class MapReduceBuilder : IDisposable
    {    
        MapReduce mr;
        public MapReduce MapReduce {
            get {
                return mr;
            }
        }
        
        public MapReduceBuilder(MapReduce mr){
            this.mr = mr;
        }
        
        /// <summary>The map function references the variable this to inspect the current object under consideration.
        /// A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        /// as may be appropriate.
        /// </summary>
        public MapReduceBuilder Map(string function){
            return this.Map(new Code(function));
        }
        
        /// <summary>The map function references the variable this to inspect the current object under consideration.
        /// A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        /// as may be appropriate.
        /// </summary>
        public MapReduceBuilder Map(Code function){
            mr.Map = function;   
            return this;
        }
        
        /// <summary>
        /// The reduce function receives a key and an array of values. To use, reduce the received values, 
        /// and return a result.
        /// </summary>
        /// <remarks>The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        /// must be idempotent. If you need to perform an operation only once, use a finalize function.</remarks>
        public MapReduceBuilder Reduce(string function){
            return this.Reduce(new Code(function));
        }
        
        /// <summary>
        /// The reduce function receives a key and an array of values. To use, reduce the received values, 
        /// and return a result.
        /// </summary>
        /// <remarks>The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        /// must be idempotent. If you need to perform an operation only once, use a finalize function.</remarks>
        public MapReduceBuilder Reduce(Code function){
            mr.Reduce = function;
            return this;
        }
        
        /// <summary>
        /// Query filter object
        /// </summary>
        public MapReduceBuilder Query(Document query){
            mr.Query = query;
            return this;
        }
        
        /// <summary>
        /// Sort the query.  Useful for optimization
        /// </summary>
        public MapReduceBuilder Sort(Document sort){
            mr.Sort = sort;   
            return this;
        }        
        
        /// <summary>
        /// Number of objects to return from collection
        /// </summary>
        public MapReduceBuilder Limit(long limit){
            mr.Limit = limit;
            return this;
        }        
        
        /// <summary>
        /// Name of the final collection the results should be stored in.
        /// </summary>
        /// <remarks>A temporary collection is still used and then renamed to the target name atomically.
        /// </remarks>
        public MapReduceBuilder Out(String name){
            mr.Out = name;
            return this;
        }
        
        /// <summary>
        /// When true the generated collection is not treated as temporary.  Specifying out automatically makes
        /// the collection permanent
        /// </summary>
        public MapReduceBuilder KeepTemp(bool keep){
            mr.KeepTemp = keep;
            return this;
        }        

        /// <summary>
        /// Provides statistics on job execution time.
        /// </summary>
        public MapReduceBuilder Verbose(bool val){
            mr.Verbose = val;
            return this;
        }
        
        /// <summary>
        /// Function to apply to all the results when finished.
        /// </summary>
        public MapReduceBuilder Finalize(Code function){
            mr.Finalize = function;   
            return this;
        }
        
        /// <summary>
        /// Document where fields go into javascript global scope
        /// </summary>
        public MapReduceBuilder Scope(Document scope){
            mr.Scope = scope;   
            return this;
        }
        
        public MapReduce Execute(){
            mr.Execute();
            return mr;
        }
        
        #region IDisposable implementation
        public void Dispose (){
            mr.Dispose();
        }
        #endregion
    }
}
