using System;
using System.Collections.Generic;
using System.IO;

using MongoDB.Driver;

namespace MongoDB.GridFS
{
    /// <summary>
    /// Provides instance methods for the creation, copying, deletion, moving, and opening of files, 
    /// and aids in the creation of GridFileStream objects.  The api is very similar to the FileInfo class in
    /// System.IO.
    /// 
    /// </summary>
    public class GridFileInfo
    {
        private const int DEFAULT_CHUNKSIZE = 256 * 1024;
        private const string DEFAULT_CONTENT_TYPE = "text/plain";
        
        private GridFile gridFile;
        private IMongoDatabase db;
        private string bucket;


        #region "filedata properties"
        private Document filedata = new Document();

        public Object Id{
            get { return filedata["_id"]; }
            set { filedata["_id"] = value; }
        }

        public string FileName
        {
            get { return (String)filedata["filename"]; }
            set { filedata["filename"] = value; }
        }

        public string ContentType{
            get { return (String)filedata["contentType"]; }
            set { filedata["contentType"] = value; }
        }

        /// <summary>
        /// Writing to the length property will not affect the actual data of the file.  Open a GridFileStream
        /// and call SetLength instead. 
        /// </summary>
        public long Length{
            get { return Convert.ToInt32(filedata["length"]); }
            set { filedata["length"] = value; }
        }

        public IList<String> Aliases{
            get {
                if(filedata.Contains("aliases") == false || filedata["aliases"] == null){
                    return null;
                }
                if(filedata["aliases"] is IList<String>){
                    return (List<String>)filedata["aliases"];
                }else{
                    return new List<String>();   
                }
            }
            set { filedata["aliases"] = value; }
        }

        public int ChunkSize{
            get { return Convert.ToInt32(filedata["chunkSize"]); }
            set { filedata["chunkSize"] = value; }
        }

        public Object Metadata{
            get { return (Document)filedata["metadata"]; }
        }

        public DateTime? UploadDate{
            get { return Convert.ToDateTime(filedata["uploadDate"]); }
            set { filedata["uploadDate"] = value; }
        }

        public string Md5{
            get { return (String)filedata["md5"]; }
            set { filedata["md5"] = value; }
        }
        #endregion

        public GridFileInfo(IMongoDatabase db, string bucket, string filename){
            this.db = db;
            this.bucket = bucket;
            this.gridFile = new GridFile(db,bucket);
            SetFileDataDefaults(filename);
            if(gridFile.Exists(filename)) this.LoadFileData();
        }

        public GridFileInfo(MongoDatabase db, string filename){
            this.db = db;
            this.bucket = "fs";
            this.gridFile = new GridFile(db);
            SetFileDataDefaults(filename);
            if(gridFile.Exists(filename)) this.LoadFileData();
        }
        
        private void SetFileDataDefaults(string filename){
            this.FileName = filename;
            this.ChunkSize = DEFAULT_CHUNKSIZE;
            this.ContentType = DEFAULT_CONTENT_TYPE;
            this.UploadDate = DateTime.UtcNow;
            this.Length = 0;
        }
        
        #region Create
        /// <summary>
        /// Creates the file named FileName and returns the GridFileStream
        /// </summary>
        /// <exception cref="IOEXception">If the file already exists</exception>
        public GridFileStream Create(){
            return Create(FileMode.CreateNew);
        }
        
        public GridFileStream Create(FileMode mode){
            return Create(mode,FileAccess.ReadWrite);
        }        
        
        public GridFileStream Create(FileMode mode, FileAccess access){
            switch (mode) {
                case FileMode.CreateNew:
                    if(gridFile.Exists(this.FileName)){
                        throw new IOException("File already exists");
                    }                    
                    this.gridFile.Files.Insert(filedata);
                    return new GridFileStream(this, this.gridFile.Files, this.gridFile.Chunks, access);
                case FileMode.Create:
                    if(gridFile.Exists(this.FileName)){
                        return this.Open(FileMode.Truncate,access);
                    }else{
                        return this.Create(FileMode.CreateNew,access);
                    }
                default:
                    throw new ArgumentException("Invalid FileMode", "mode");
            }
        }
        #endregion
        
        #region Open
        /// <summary>
        /// Creates a read-only GridFileStream to an existing file. 
        /// </summary>
        /// <returns></returns>
        public GridFileStream OpenRead(){
            return this.Open(FileMode.Open, FileAccess.Read);
        }
        
        /// <summary>
        /// Creates a write-only GridFileStream to an existing file.
        /// </summary>
        /// <returns></returns>
        public GridFileStream OpenWrite(){
            return this.Open(FileMode.Open, FileAccess.Write);
        }
        
        public GridFileStream Open(FileMode mode, FileAccess access){
            switch (mode) {
                case FileMode.Create:
                    if(gridFile.Exists(this.FileName) == true){
                        return this.Open(FileMode.Truncate, access);
                    }else{
                        return this.Create(FileMode.CreateNew, access);
                    }
                case FileMode.CreateNew:
                    return this.Create(mode, access);
                case FileMode.Open:
                    LoadFileData();
                    return new GridFileStream(this,this.gridFile.Files, this.gridFile.Chunks, access);
                case FileMode.OpenOrCreate:
                    if(gridFile.Exists(this.FileName) == false) return this.Create(mode, access);
                    LoadFileData();
                    return new GridFileStream(this, this.gridFile.Files, this.gridFile.Chunks, access);
                case FileMode.Truncate:
                    this.Truncate();
                    return new GridFileStream(this,this.gridFile.Files, this.gridFile.Chunks, access);
                case FileMode.Append:
                    LoadFileData();
                    GridFileStream gfs = new GridFileStream(this,this.gridFile.Files, this.gridFile.Chunks, access);
                    gfs.Seek(0,SeekOrigin.End);
                    return gfs;
            }
            throw new NotImplementedException("Mode not implemented.");
        }
        #endregion
        
        /// <summary>
        /// Permanently removes a file from the database. 
        /// </summary>
        public void Delete(){
            if(this.Id != null){
                this.gridFile.Delete(this.Id);
            }else{
                this.gridFile.Delete(this.FileName);
            }
        }
        
        /// <summary>
        /// Renames a file. 
        /// </summary>
        public void MoveTo(String newFileName){
            this.gridFile.Move(this.FileName, newFileName);
            this.FileName = newFileName;
        }
        
        /// <summary>
        /// Gets a value indicating whether the file exists.
        /// </summary>
        public Boolean Exists{
            get{
                return this.gridFile.Exists(this.FileName);
            }
        }
        
        /// <summary>
        /// Deletes all data in a file and sets the length to 0.
        /// </summary>
        public void Truncate(){
            if(filedata.Contains("_id") == false) return;
            this.gridFile.Chunks.Delete(new Document().Add("files_id", filedata["_id"]));
            this.Length = 0;
            this.gridFile.Files.Update(filedata);
        }

        public string CalcMD5(){
            Document doc = this.db.SendCommand(new Document().Add("filemd5", this.Id).Add("root", this.bucket));
            return (String)doc["md5"];
        }
        
        /// <summary>
        /// Updates the aliases, contentType, metadata and uploadDate in the database.
        /// </summary>
        /// <remarks> To rename a file use the MoveTo method.
        /// </remarks>
        public void UpdateInfo(){
            Document info = new Document(){{"uploadDate", this.UploadDate},
                                            {"aliases", this.Aliases}, 
                                            {"metadata", this.Metadata},
                                            {"contentType", this.ContentType}};
            this.gridFile.Files.Update(new Document(){{"$set",info}}, new Document(){{"_id", this.Id}});
        }
        
        /// <summary>
        /// Reloads the file information from the database. 
        /// </summary>
        /// <remarks>The data in the database will not reflect any changes done through an open stream until it is closed.
        /// </remarks>
        public void Refresh(){
            LoadFileData();
        }
        
        private void LoadFileData(){
            Document doc = this.gridFile.Files.FindOne(new Document().Add("filename", this.FileName));
            if(doc != null){
                filedata = doc;
            }else{
                throw new DirectoryNotFoundException(this.gridFile.Name + Path.VolumeSeparatorChar + this.FileName);
            }
        }
        
        public Document ToDocument(){
           return this.filedata;
       }
        
        public override string ToString(){
            return filedata.ToString();
        }
        
    }
}
