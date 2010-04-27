namespace MongoDB.Driver
{
    /// <summary>
    /// Query options
    /// </summary>
    /// <remarks>
    /// Oplog replay: 8 (internal replication use only - drivers should not implement)
    /// </remarks>
    public enum QueryOptions {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Tailable cursor
        /// </summary>
        TailableCursor = 2,
        /// <summary>
        /// Slave OK
        /// </summary>
        SlaveOK = 4,
        /// <summary>
        /// No cursor timeout
        /// </summary>
        NoCursorTimeout = 16
    }
}