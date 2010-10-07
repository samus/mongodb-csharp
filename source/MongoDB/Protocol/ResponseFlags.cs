using System;

namespace MongoDB.Protocol
{
    /// <summary>
    ///   Server response flags
    /// </summary>
    /// <remarks>
    ///   ShardConfigStale = 4 is ignored because its Mongo internal.
    /// </remarks>
    [Flags]
    internal enum ResponseFlags
    {
        /// <summary>
        /// </summary>
        None = 0,
        /// <summary>
        ///   Set when getMore is called but the cursor id is not valid at 
        ///   the server. Returned with zero results.
        /// </summary>
        CursorNotFound = 1,
        /// <summary>
        ///   Set when query failed. Results consist of one document containing 
        ///   an "$err" field describing the failure.
        /// </summary>
        QueryFailure = 2,
        /// <summary>
        ///   Set when the server supports the AwaitData Query option. If it 
        ///   doesn't, a client should sleep a little between getMore's of a 
        ///   Tailable cursor. Mongod version 1.6 supports AwaitData and 
        ///   thus always sets AwaitCapable.
        /// </summary>
        AwaitCapable = 8
    }
}