using System;

namespace MongoDB.GridFS
{
    public class MongoGridFSException : Exception
    {
        private string filename;
        public string Filename
        {
            get { return filename; }
        }

        public MongoGridFSException(string message, string filename, Exception inner)
            : base(message, inner)
        {
            this.filename = filename;
        }
    }    
}
