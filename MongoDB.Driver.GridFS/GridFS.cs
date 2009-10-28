using System;
using System.IO;
using MongoDB.Driver;

namespace MongoDB.Driver.GridFS
{
    public class GridFS
    {
        private const int DEFAULT_CHUNKSIZE = 256 * 1024;
        private const string DEFAULT_ROOT = "fs";
        private Mongo mongo;
        Collection chunks;
        Collection files;

        #region Ctors
        public GridFS(Mongo mongo){
            this.mongo = mongo;
            this.bucketName = DEFAULT_ROOT;
            chunks = this.mongo[this.bucketName][".chunks"];
            files = this.mongo[this.bucketName][".files"];
        }

        public GridFS(Mongo mongo, string bucketName){
            this.mongo = mongo;
            this.bucketName = bucketName;
            chunks = this.mongo[this.bucketName][".chunks"];
            files = this.mongo[this.bucketName][".files"];
            
        }

        #endregion
       
        public void StoreFile()
        {
           Int32[] i = new Int32[DEFAULT_CHUNKSIZE];
        }

        #region Properties
        private string bucketName;
              
        #endregion
    }


}
