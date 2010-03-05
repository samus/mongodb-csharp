using System;

namespace MongoDB.Driver
{
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
}