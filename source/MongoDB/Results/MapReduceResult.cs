using System;

namespace MongoDB.Results
{
    /// <summary>
    /// </summary>
    public class MapReduceResult
    {
        private readonly Document _counts;
        private readonly Document _result;
        private TimeSpan _timeSpan = TimeSpan.Zero;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MapReduceResult" /> class.
        /// </summary>
        /// <param name = "result">The result.</param>
        public MapReduceResult(Document result)
        {
            if(result == null)
                throw new ArgumentNullException("result");

            _result = result;
            _counts = (Document)result["counts"];
        }

        /// <summary>
        ///   Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public string CollectionName
        {
            get { return (string)_result["result"]; }
        }

        /// <summary>
        ///   Gets the input count.
        /// </summary>
        /// <value>The input count.</value>
        public long InputCount
        {
            get { return Convert.ToInt64(_counts["input"]); }
        }

        /// <summary>
        ///   Gets the emit count.
        /// </summary>
        /// <value>The emit count.</value>
        public long EmitCount
        {
            get { return Convert.ToInt64(_counts["emit"]); }
        }

        /// <summary>
        ///   Gets the output count.
        /// </summary>
        /// <value>The output count.</value>
        public long OutputCount
        {
            get { return Convert.ToInt64(_counts["output"]); }
        }

        /// <summary>
        ///   Gets the time.
        /// </summary>
        /// <value>The time.</value>
        public long Time
        {
            get { return Convert.ToInt64(_result["timeMillis"]); }
        }

        /// <summary>
        ///   Gets the time span.
        /// </summary>
        /// <value>The time span.</value>
        public TimeSpan TimeSpan
        {
            get
            {
                if(_timeSpan == TimeSpan.Zero)
                    _timeSpan = TimeSpan.FromMilliseconds(Time);
                return _timeSpan;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "MapReduceResult" /> is ok.
        /// </summary>
        /// <value><c>true</c> if ok; otherwise, <c>false</c>.</value>
        public Boolean Ok
        {
            get { return (Convert.ToBoolean(_result["ok"])); }
        }

        /// <summary>
        ///   Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public String ErrorMessage
        {
            get
            {
                if(_result.Contains("msg"))
                    return (String)_result["msg"];
                return String.Empty;
            }
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _result.ToString();
        }
    }
}