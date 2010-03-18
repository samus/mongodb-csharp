namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// Base class for all raw messages
    /// </summary>
    public abstract class MessageBase
    {        protected const int MaximumMessageSize = 1024 * 1024 * 4;
        public MessageHeader Header { get; protected set; }
    }
}
