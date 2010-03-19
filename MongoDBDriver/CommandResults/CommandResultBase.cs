using MongoDB.Driver.Serialization.Attributes;

namespace MongoDB.Driver.CommandResults
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CommandResultBase
    {
        /// <summary>
        /// Gets or sets the server error message.
        /// </summary>
        /// <value>The error message.</value>
        [MongoName("errmsg")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CommandResultBase"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        [MongoName("ok")]
        public bool Success { get; set; }
    }
}