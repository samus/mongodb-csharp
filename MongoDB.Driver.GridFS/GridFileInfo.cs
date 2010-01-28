using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MongoDB.Driver.GridFS
{
    /// <summary>
    /// Provides instance methods for the creation, copying, deletion, moving, and opening of files, 
    /// and aids in the creation of GridFileStream objects.  It also contains 
    /// </summary>
    public sealed class GridFileInfo
    {
        private const int DEFAULT_CHUNKSIZE = 256 * 1024;
        private const string DEFAULT_CONTENT_TYPE = "text/plain";
        
        private GridFile gridFile;


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

        public long Length{
            get { return Convert.ToInt32(filedata["length"]); }
            set { filedata["length"] = value; }
        }

        public string[] Aliases{
            get { return (String[])filedata["aliases"]; }
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

        public GridFileInfo(Database db, string bucket, string filename){
            this.gridFile = new GridFile(db,bucket);
            SetFileDataDefaults(filename);
            if(gridFile.Exists(filename)) this.LoadFileData();
        }

        public GridFileInfo(Database db, string filename){
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
                case FileMode.CreateNew:
                    return this.Create(mode, access);
                case FileMode.Truncate:
                    this.Truncate();
                    return new GridFileStream(this,this.gridFile.Files, this.gridFile.Chunks, access);
                case FileMode.Open:
                    LoadFileData();
                    return new GridFileStream(this,this.gridFile.Files, this.gridFile.Chunks, access);
            }
            throw new NotImplementedException("Not all modes are implemented yet.");
        }
        
        public void Delete(){
            if(this.Id != null){
                this.gridFile.Delete(this.Id);
            }else{
                this.gridFile.Delete(this.FileName);
            }
        }
        
        public void MoveTo(String newFileName){
            this.gridFile.Move(this.FileName, newFileName);
        }
        
        public void Truncate(){
            if(filedata.Contains("_id") == false) return;
            this.gridFile.Chunks.Delete(new Document().Append("files_id", filedata["_id"]));
            this.Length = 0;
            this.gridFile.Files.Update(filedata);
        }
        
        private void LoadFileData(){
            Document doc = this.gridFile.Files.FindOne(new Document().Append("filename",this.FileName));
            if(doc != null){
                filedata = doc;
            }else{
                throw new DirectoryNotFoundException(this.FileName);
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
