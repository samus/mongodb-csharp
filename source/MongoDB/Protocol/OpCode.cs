namespace MongoDB.Protocol
{
    /// <summary>
    /// </summary>
    internal enum OpCode
    {
        /// <summary>
        ///   Reply to a client request. responseTo is set.
        /// </summary>
        Reply = 1,
        /// <summary>
        ///   Generic msg command followed by a string.
        /// </summary>
        Msg = 1000,
        /// <summary>
        ///   update document
        /// </summary>
        Update = 2001,
        /// <summary>
        ///   insert new document
        /// </summary>
        Insert = 2002,
        /// <summary>
        ///   is this used?
        /// </summary>
        GetByOid = 2003,
        /// <summary>
        ///   query a collection
        /// </summary>
        Query = 2004,
        /// <summary>
        ///   Get more data from a query. See Cursors.
        /// </summary>
        GetMore = 2005,
        /// <summary>
        ///   Delete documents
        /// </summary>
        Delete = 2006,
        /// <summary>
        ///   Tell database client is done with a cursor.
        /// </summary>
        KillCursors = 2007
    }
}