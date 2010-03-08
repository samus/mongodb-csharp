namespace MongoDB.Driver.Protocol
{
    /// <summary>
    /// Base class for all raw messages
    /// </summary>
    public abstract class MessageBase
    {
        public MessageHeader Header { get; set; }
    }
}
