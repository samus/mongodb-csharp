using MongoDB.Attributes;

namespace MongoDB.Results
{
    internal class FindAndModifyResult<T> : CommandResultBase
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [MongoAlias("value")]
        public T Value { get; set; }
    }
}