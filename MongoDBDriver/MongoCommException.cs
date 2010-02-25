using System;

namespace MongoDB.Driver
{
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
}