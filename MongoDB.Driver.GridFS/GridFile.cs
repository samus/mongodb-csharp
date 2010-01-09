using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace MongoDB.Driver.GridFS
{
    public class GridFile{

        private Database db;
        
        private string name;
        
        public string Name {
            get { return name; }
        }
        
        private IMongoCollection files;
        public IMongoCollection Files{
            get { return this.files; }
        }

        private IMongoCollection chunks;
        public IMongoCollection Chunks{
            get { return this.chunks; }
        }        
        
        public GridFile(Database db){
            this.db = db;
            this.files = db["fs.files"];
            this.chunks = db["fs.chunks"];
            this.name = "fs";
        }

        public GridFile(Database db, string bucket){
            this.db = db;
            this.files = db[bucket + ".files"];
            this.chunks = db[bucket + ".chunks"];
            this.name = "fs";
        }
        
        public ICursor ListFiles(){
            return this.ListFiles(new Document());
        }
        
        public ICursor ListFiles(Document query){
            return this.files.Find(new Document().Append("query",query).Append("orderby", new Document().Append("filename", 1)));
        }
        
        public Document Copy(String src, String dest){
            throw new NotImplementedException("Copy");
        }
        
        public Document Create(String name){
            throw new NotImplementedException("Create");
        }
        
        #region Delete
        public void Delete(Oid id ){
            files.Delete(new Document().Append("_id",id));
            chunks.Delete(new Document().Append("files_id",id));
        }
        
        public void Delete( String filename ){
            files.Delete(new Document().Append("filename",filename));
        }
        
        public void Delete(Document query ){
            foreach(Document doc in ListFiles(query).Documents){
                Delete((Oid)doc["_id"]);
            }
        }
        #endregion
        
        #region Exists        
        public Boolean Exists(string name){
            return this.files.FindOne(new Document().Append("filename",name)) != null;
        }

        public Boolean Exists(Oid id){
            return this.files.FindOne(new Document().Append("_id",id)) != null;
        }
        
        #endregion        
        
        
        //public void StoreFile(string filepath)
        //{
        //    if (File.Exists(filepath))
        //    {
        //        fileStream = new FileStream(filepath, FileMode.Open);
        //        binaryReader = new BinaryReader(fileStream);
        //        int chunkNumber = 0;
        //        int offset = 0;
        //        int lastSize = (int)fileStream.Length % this.gridFile.ChunkSize;
        //        double nthChunk = 0;
        //        if (fileStream.Length > gridFile.ChunkSize)
        //        {
        //            nthChunk = Math.Ceiling(fileStream.Length / (double)gridFile.ChunkSize);
        //        }
        //        while (offset < fileStream.Length)
        //        {
        //            byte[] data = new byte[gridFile.ChunkSize];
        //            if (chunkNumber < nthChunk)
        //            {
        //                data = binaryReader.ReadBytes(gridFile.ChunkSize);
        //            }
        //            else
        //            {
        //                data = binaryReader.ReadBytes(lastSize);
        //            }
                 
        //            gridFile.Chunks.Add(new GridChunk(gridFile.Id, chunkNumber, data));
        //            offset += gridFile.ChunkSize;
        //            chunkNumber++;
        //        }
        //    }
        //    else
        //    {
        //        throw new IOException("This file does not exist.");
        //    }
        //}

    
    }

}
