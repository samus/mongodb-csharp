namespace MongoDB.Driver.Results
{
    internal class FindAndModifyResult<T> : CommandResultBase
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; set; }
    }
}