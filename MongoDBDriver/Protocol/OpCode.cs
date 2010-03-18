namespace MongoDB.Driver.Protocol
{
    public enum OpCode{
        Reply = 1, //Reply to a client request. responseTo is set
        Msg = 1000, //generic msg command followed by a string
        Update = 2001, //update document
        Insert = 2002, //insert new document
        GetByOid = 2003, //is this used?
        Query = 2004, //query a collection
        GetMore = 2005, //Get more data from a query. See Cursors
        Delete = 2006, //Delete documents
        KillCursors = 2007 //Tell database client is done with a cursor         
    }
}