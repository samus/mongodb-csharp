using System;

namespace MongoDB.Driver
{
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
}