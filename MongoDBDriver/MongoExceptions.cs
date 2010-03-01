using System;

namespace MongoDB.Driver 
{
    /// <summary>
    /// Base class for all Mongo Exceptions
    /// </summary>
    public class MongoException : Exception
    {
        public MongoException(string message, Exception inner):base(message,inner){}
        public MongoException(string message):base(message){}
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
    
    public class MongoOperationException : MongoException
    {
        private Document error;
        public Document Error {
            get {return error;}
        }
        public MongoOperationException(string message, Document error):this(message, error,null){}
        public MongoOperationException(string message, Document error, Exception e):base(message,e){
            this.error = error;
        }        
    }
    /// <summary>
    /// Raised when an action causes a unique constraint violation in an index. 
    /// </summary>
    public class MongoDuplicateKeyException : MongoOperationException
    {
        public MongoDuplicateKeyException(string message, Document error):base(message, error,null){}
        public MongoDuplicateKeyException(string message, Document error, Exception e):base(message, error,e){}
    }
    
    /// <summary>
    /// Raised when an update action causes a unique constraint violation in an index.
    /// </summary>
    /// <remarks>
    /// It is only another class because Mongo makes a distinction and it may be helpful.
    /// </remarks>
    public class MongoDuplicateKeyUpdateException : MongoDuplicateKeyException
    {
        public MongoDuplicateKeyUpdateException(string message, Document error)
                :base(message,error){}
        public MongoDuplicateKeyUpdateException(string message, Document error, Exception e):base(message, error,e){} 
    }
    
    /// <summary>
    /// Raised when a command returns a failure message. 
    /// </summary>
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
        public MongoCommandException(string message, Document error, Document command, Exception e):base(message,e){
            this.error = error;
            this.command = command;
        }        
    }
    
    /// <summary>
    /// Raised when a map reduce call fails. 
    /// </summary>
    public class MongoMapReduceException : MongoCommandException
    {
        private MapReduce.MapReduceResult mrr;
        public MapReduce.MapReduceResult MapReduceResult{
            get{return mrr;}
        }
        
        public MongoMapReduceException(MongoCommandException mce, MapReduce mr):base(mce.Message,mce.Error, mce.Command){
            mrr = new MapReduce.MapReduceResult(mce.Error);
        }
    }
}
