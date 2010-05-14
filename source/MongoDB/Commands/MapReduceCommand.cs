using System;

namespace MongoDB.Commands
{
    /// <summary>
    ///   A fluent interface for executing a Map/Reduce call against a collection.
    /// </summary>
    public class MapReduceCommand
    {
        internal Document Command { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapReduceCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        internal MapReduceCommand(string name)
        {
            Command = new Document("mapreduce", name);
            Verbose = true;
        }

        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return (String)Command["mapreduce"]; }
        }

        /// <summary>
        ///   The map function references the variable this to inspect the current object under consideration.
        ///   A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        ///   as may be appropriate.
        /// </summary>
        public Code Map
        {
            get { return (Code)Command["map"]; }
            set { Command["map"] = value; }
        }

        /// <summary>
        ///   The reduce function receives a key and an array of values. To use, reduce the received values, 
        ///   and return a result.
        /// </summary>
        /// <remarks>
        ///   The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        ///   must be idempotent. If you need to perform an operation only once, use a finalize function.
        /// </remarks>
        public Code Reduce
        {
            get { return (Code)Command["reduce"]; }
            set { Command["reduce"] = value; }
        }

        /// <summary>
        ///   Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public Document Query
        {
            get { return (Document)Command["query"]; }
            set { Command["query"] = value; }
        }

        /// <summary>
        ///   Sort the query.  Useful for optimization
        /// </summary>
        public Document Sort
        {
            get { return (Document)Command["sort"]; }
            set { Command["sort"] = value; }
        }

        /// <summary>
        ///   Number of objects to return from collection
        /// </summary>
        public long Limit
        {
            get { return (long)Command["limit"]; }
            set { Command["limit"] = value; }
        }

        /// <summary>
        ///   Name of the final collection the results should be stored in.
        /// </summary>
        /// <remarks>
        ///   A temporary collection is still used and then renamed to the target name atomically.
        /// </remarks>
        public string Out
        {
            get { return (string)Command["out"]; }
            set { Command["out"] = value; }
        }

        /// <summary>
        ///   When true the generated collection is not treated as temporary.  Specifying out automatically makes
        ///   the collection permanent
        /// </summary>
        public bool KeepTemp
        {
            get { return Convert.ToBoolean(Command["keeptemp"]); }
            set { Command["keeptemp"] = value; }
        }

        /// <summary>
        ///   Provides statistics on job execution time.
        /// </summary>
        public bool Verbose
        {
            get { return (bool)Command["verbose"]; }
            set { Command["verbose"] = value; }
        }

        /// <summary>
        ///   Function to apply to all the results when finished.
        /// </summary>
        public Code Finalize
        {
            get { return (Code)Command["finalize"]; }
            set { Command["finalize"] = value; }
        }

        /// <summary>
        ///   Document where fields go into javascript global scope
        /// </summary>
        public Document Scope
        {
            get { return (Document)Command["scope"]; }
            set { Command["scope"] = value; }
        }
    }
}