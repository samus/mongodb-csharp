namespace MongoDB.Protocol
{
    /// <summary>
    /// Base class for all raw messages
    /// </summary>
    internal abstract class MessageBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected const int MaximumMessageSize = 1024 * 1024 * 4;

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public MessageHeader Header { get; protected set; }
    }
}
