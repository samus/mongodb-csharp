using System;
using System.Collections.Generic;

namespace MongoDB
{
    /// <summary>
    ///   A fluent interface for executing a Map/Reduce call against a collection.
    /// </summary>
    public class MapReduce : IDisposable
    {
        private readonly Document command;
        private readonly IMongoDatabase database;
        private bool canModify = true;
        private bool disposing;

        internal MapReduce(IMongoDatabase database, string name){
            this.database = database;
            command = new Document().Add("mapreduce", name);
            Verbose = true;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name{
            get { return (String)command["mapreduce"]; }
        }

        /// <summary>
        ///   Holds the resulting value of the execution.
        /// </summary>
        public MapReduceResult Result { get; private set; }

        /// <summary>
        ///   The map function references the variable this to inspect the current object under consideration.
        ///   A map function must call emit(key,value) at least once, but may be invoked any number of times, 
        ///   as may be appropriate.
        /// </summary>
        public Code Map{
            get { return (Code)command["map"]; }
            set{
                TryModify();
                command["map"] = value;
            }
        }

        /// <summary>
        ///   The reduce function receives a key and an array of values. To use, reduce the received values, 
        ///   and return a result.
        /// </summary>
        /// <remarks>
        ///   The MapReduce engine may invoke reduce functions iteratively; thus, these functions 
        ///   must be idempotent. If you need to perform an operation only once, use a finalize function.
        /// </remarks>
        public Code Reduce{
            get { return (Code)command["reduce"]; }
            set{
                TryModify();
                command["reduce"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public Document Query{
            get { return (Document)command["query"]; }
            set{
                TryModify();
                command["query"] = value;
            }
        }

        /// <summary>
        ///   Sort the query.  Useful for optimization
        /// </summary>
        public Document Sort{
            get { return (Document)command["sort"]; }
            set{
                TryModify();
                command["sort"] = value;
            }
        }

        /// <summary>
        ///   Number of objects to return from collection
        /// </summary>
        public long Limit{
            get { return (long)command["limit"]; }
            set{
                TryModify();
                command["limit"] = value;
            }
        }

        /// <summary>
        ///   Name of the final collection the results should be stored in.
        /// </summary>
        /// <remarks>
        ///   A temporary collection is still used and then renamed to the target name atomically.
        /// </remarks>
        public string Out{
            get { return (string)command["out"]; }
            set{
                TryModify();
                command["out"] = value;
            }
        }

        /// <summary>
        ///   When true the generated collection is not treated as temporary.  Specifying out automatically makes
        ///   the collection permanent
        /// </summary>
        public bool KeepTemp{
            get { return Convert.ToBoolean(command["keeptemp"]); }
            set{
                TryModify();
                command["keeptemp"] = value;
            }
        }

        /// <summary>
        ///   Provides statistics on job execution time.
        /// </summary>
        public bool Verbose{
            get { return (bool)command["verbose"]; }
            set{
                TryModify();
                command["verbose"] = value;
            }
        }

        /// <summary>
        ///   Function to apply to all the results when finished.
        /// </summary>
        public Code Finalize{
            get { return (Code)command["finalize"]; }
            set{
                TryModify();
                command["finalize"] = value;
            }
        }

        /// <summary>
        ///   Document where fields go into javascript global scope
        /// </summary>
        public Document Scope{
            get { return (Document)command["scope"]; }
            set{
                TryModify();
                command["scope"] = value;
            }
        }

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public IEnumerable<Document> Documents{
            get{
                if(Result == null)
                    Execute();
                if(Result.Ok == false)
                    throw new InvalidOperationException("Documents cannot be iterated when an error was returned from execute.");

                var docs = database[Result.CollectionName].FindAll().Documents;
                using((IDisposable)docs){
                    foreach(var doc in docs)
                        yield return doc;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can modify.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can modify; otherwise, <c>false</c>.
        /// </value>
        public Boolean CanModify{
            get { return canModify; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){
            if(KeepTemp || Out != null || disposing)
                return;
            disposing = true;

            if(Result == null || Result.Ok == false)
                return; //Nothing to do.

            //Drop the temporary collection that was created as part of results.
            database.Metadata.DropCollection(Result.CollectionName);
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public MapReduce Execute(){
            if(command.Contains("map") == false || command.Contains("reduce") == false)
                throw new InvalidOperationException("Cannot execute without a map and reduce function");
            canModify = false;
            try{
                Result = new MapReduceResult(database.SendCommand(command));
            }
            catch(MongoCommandException mce){
                Result = new MapReduceResult(mce.Error);
                throw new MongoMapReduceException(mce, this);
            }
            return this;
        }

        internal void TryModify(){
            if(canModify == false)
                throw new InvalidOperationException("Cannot modify a map/reduce that has already executed");
        }

        /// <summary>
        /// 
        /// </summary>
        public class MapReduceResult
        {
            private readonly Document counts;
            private readonly Document result;
            private TimeSpan span = TimeSpan.MinValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="MapReduceResult"/> class.
            /// </summary>
            /// <param name="result">The result.</param>
            public MapReduceResult(Document result){
                this.result = result;
                counts = (Document)result["counts"];
            }

            /// <summary>
            /// Gets the name of the collection.
            /// </summary>
            /// <value>The name of the collection.</value>
            public string CollectionName{
                get { return (string)result["result"]; }
            }

            /// <summary>
            /// Gets the input count.
            /// </summary>
            /// <value>The input count.</value>
            public long InputCount{
                get { return Convert.ToInt64(counts["input"]); }
            }

            /// <summary>
            /// Gets the emit count.
            /// </summary>
            /// <value>The emit count.</value>
            public long EmitCount{
                get { return Convert.ToInt64(counts["emit"]); }
            }

            /// <summary>
            /// Gets the output count.
            /// </summary>
            /// <value>The output count.</value>
            public long OutputCount{
                get { return Convert.ToInt64(counts["output"]); }
            }

            /// <summary>
            /// Gets the time.
            /// </summary>
            /// <value>The time.</value>
            public long Time{
                get { return Convert.ToInt64(result["timeMillis"]); }
            }

            /// <summary>
            /// Gets the time span.
            /// </summary>
            /// <value>The time span.</value>
            public TimeSpan TimeSpan{
                get{
                    if(span == TimeSpan.MinValue)
                        span = TimeSpan.FromMilliseconds(Time);
                    return span;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="MapReduceResult"/> is ok.
            /// </summary>
            /// <value><c>true</c> if ok; otherwise, <c>false</c>.</value>
            public Boolean Ok{
                get { return (1.0 == Convert.ToDouble(result["ok"])); }
            }

            /// <summary>
            /// Gets the error message.
            /// </summary>
            /// <value>The error message.</value>
            public String ErrorMessage{
                get{
                    if(result.Contains("msg"))
                        return (String)result["msg"];
                    return String.Empty;
                }
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString(){
                return result.ToString();
            }
        }
    }
}