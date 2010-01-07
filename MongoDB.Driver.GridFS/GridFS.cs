using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace MongoDB.Driver.GridFS
{
    public class GridFS
    {
        public GridFS(Mongo mongo)
        {
            this.mongo = mongo;
            this.files = mongo["fs"]["files"];
            this.chunks = mongo["fs"]["chunks"];
        }

        public GridFS(Mongo mongo, string collection){
            this.mongo = mongo;
            this.files = mongo["fs"][collection];
            this.chunks = mongo["fs"][collection];
        }
        
        private Mongo mongo;
        public Mongo Mongo
        {
            get { return this.mongo; }
        }
        private IMongoCollection files;
        public IMongoCollection Files
        {
            get { return this.files; }
        }

        private IMongoCollection chunks;
        public IMongoCollection Chunks
        {
            get { return this.chunks; }
        }
    
    }

}
