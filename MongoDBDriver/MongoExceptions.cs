using System;

namespace MongoDB.Driver 
{
    /// <summary>
    /// Base class for all Mongo Exceptions
    /// </summary>
    public class MongoException : Exception
    {
        public MongoException(string message, Exception inner):base(message,inner){}
    }
    
    public class MongoCommException : MongoException
    {
        private string host;
        public string Host {
            get { return host; }
        }
        
        private int port;       
        public int Port {
            get { return port; }
        }
        
        public MongoCommException(string message, Connection conn):this(message,conn,null){}
        public MongoCommException(string message, Connection conn, Exception inner):base(message,inner){
            this.host = conn.Host;
            this.port = conn.Port;          
        }       
    }
    
    public class MongoCommandException : MongoException
    {
        private Document error;
        public Document Error {
            get {return error;}
        }
        
        private Document command;
        public Document Command{
            get {return command;}
        }
        
        public MongoCommandException(string message, Document error, Document command):base(message,null){
            this.error = error;
            this.command = command;
        }
    }
}
