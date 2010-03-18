namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// Base class for all raw messages
    /// </summary>
    public abstract class MessageBase
    {
        public static int MaximumMessageSize = 1024 * 1024 * 4;
        public MessageHeader Header { get; protected set; }

    }
}
