using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace MongoDB.Driver.GridFS
{
    public class GridFile{
        
        private const int DEFAULT_CHUNKSIZE = 256 * 1024;
        private const string DEFAULT_CONTENT_TYPE = "text/plain";
        
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
        
        public GridFile(Database db):this(db,"fs"){}

        public GridFile(Database db, string bucket){
            this.db = db;
            this.files = db[bucket + ".files"];
            this.chunks = db[bucket + ".chunks"];
            this.name = bucket;
        }
        
        public ICursor ListFiles(){
            return this.ListFiles(new Document());
        }
        
        public ICursor ListFiles(Document query){
            return this.files.Find(new Document().Append("query",query).Append("orderby", new Document().Append("filename", 1)));
        }
        
        public void Copy(String src, String dest){
            //Is there away to do this server side instead of reading the file down local and then writing it back?
            throw new NotImplementedException("Copy");
        }
        
        #region Create
        public GridFileStream Create(String filename){
            return Create(name, FileMode.Create);
        }
        
        public GridFileStream Create(String filename, FileMode mode){
            return Create(name,mode,FileAccess.ReadWrite);
        }
        
        public GridFileStream Create(String filename, FileMode mode, FileAccess access){
            //Create is delegated to a GridFileInfo because the stream needs access to the gfi and it
            //is easier to do it this way and only write the implementation once.
            GridFileInfo gfi = new GridFileInfo(this.db,this.name,filename);
            gfi.Create(mode,access);
        }
        #endregion
        
        #region Delete
        public void Delete(Object id){
            files.Delete(new Document().Append("_id",id));
            chunks.Delete(new Document().Append("files_id",id));
        }
        
        public void Delete(String filename){
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

        public Boolean Exists(Object id){
            return this.files.FindOne(new Document().Append("_id",id)) != null;
        }
        #endregion        
        
        #region Move
        public void Move(String src, String dest){
            this.files.Update(new Document().Append("$set", new Document().Append("filename",dest)), new Document().Append("filename", src));
        }
        
        public void Move(Object id, String dest){
            this.files.Update(new Document().Append("$set", new Document().Append("filename",dest)), new Document().Append("_id", id));
        }
        #endregion      
    
    }

}
