using System;

namespace MongoDB.Driver
{
    /// <summary>
    ///   Provides a Fluent interface to build and execute Map/Reduce calls.
    /// </summary>
    public class MapReduceBuilder : IDisposable
    {
        private readonly MapReduce _mapReduce;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapReduceBuilder"/> class.
        /// </summary>
        /// <param name="mapReduce">The map reduce.</param>
        public MapReduceBuilder(MapReduce mapReduce){
            _mapReduce = mapReduce;
        }

        /// <summary>
        /// Gets the map reduce.
        /// </summary>
        /// <value>The map reduce.</value>
        public MapReduce MapReduce{
            get { return _mapReduce; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){
            _mapReduce.Dispose();
        }

        /// <summary>
        ///   The map function references the variable this to inspect the current object under consideration.
        ///   A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        ///   as may be appropriate.
        /// </summary>
        public MapReduceBuilder Map(string function){
            return Map(new Code(function));
        }

        /// <summary>
        ///   The map function references the variable this to inspect the current object under consideration.
        ///   A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        ///   as may be appropriate.
        /// </summary>
        public MapReduceBuilder Map(Code function){
            _mapReduce.Map = function;
            return this;
        }

        /// <summary>
        ///   The reduce function receives a key and an array of values. To use, reduce the received values, 
        ///   and return a result.
        /// </summary>
        /// <remarks>
        ///   The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        ///   must be idempotent. If you need to perform an operation only once, use a finalize function.
        /// </remarks>
        public MapReduceBuilder Reduce(string function){
            return Reduce(new Code(function));
        }

        /// <summary>
        ///   The reduce function receives a key and an array of values. To use, reduce the received values, 
        ///   and return a result.
        /// </summary>
        /// <remarks>
        ///   The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        ///   must be idempotent. If you need to perform an operation only once, use a finalize function.
        /// </remarks>
        public MapReduceBuilder Reduce(Code function){
            _mapReduce.Reduce = function;
            return this;
        }

        /// <summary>
        ///   Query filter object
        /// </summary>
        public MapReduceBuilder Query(Document query){
            _mapReduce.Query = query;
            return this;
        }

        /// <summary>
        ///   Sort the query.  Useful for optimization
        /// </summary>
        public MapReduceBuilder Sort(Document sort){
            _mapReduce.Sort = sort;
            return this;
        }

        /// <summary>
        ///   Number of objects to return from collection
        /// </summary>
        public MapReduceBuilder Limit(long limit){
            _mapReduce.Limit = limit;
            return this;
        }

        /// <summary>
        ///   Name of the final collection the results should be stored in.
        /// </summary>
        /// <remarks>
        ///   A temporary collection is still used and then renamed to the target name atomically.
        /// </remarks>
        public MapReduceBuilder Out(String name){
            _mapReduce.Out = name;
            return this;
        }

        /// <summary>
        ///   When true the generated collection is not treated as temporary.  Specifying out automatically makes
        ///   the collection permanent
        /// </summary>
        public MapReduceBuilder KeepTemp(bool keep){
            _mapReduce.KeepTemp = keep;
            return this;
        }

        /// <summary>
        ///   Provides statistics on job execution time.
        /// </summary>
        public MapReduceBuilder Verbose(bool val){
            _mapReduce.Verbose = val;
            return this;
        }

        /// <summary>
        ///   Function to apply to all the results when finished.
        /// </summary>
        public MapReduceBuilder Finalize(Code function){
            _mapReduce.Finalize = function;
            return this;
        }

        /// <summary>
        ///   Document where fields go into javascript global scope
        /// </summary>
        public MapReduceBuilder Scope(Document scope){
            _mapReduce.Scope = scope;
            return this;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public MapReduce Execute(){
            _mapReduce.Execute();
            return _mapReduce;
        }
    }
}