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

        private FileMode fileMode;
        private BinaryReader binaryReader;
        private FileStream fileStream;
        private byte[] buffer;
        private bool disposed = false;
        private bool isOpen = false;
        
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
            this.FileName = filename;
            this.ChunkSize = DEFAULT_CHUNKSIZE;
            if(gridFile.Exists(filename)) this.LoadFileData();
        }
        public GridFileInfo(Database db, string filename){
            this.gridFile = new GridFile(db);
            this.FileName = filename;
            this.ChunkSize = DEFAULT_CHUNKSIZE;
            if(gridFile.Exists(filename)) this.LoadFileData();
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
                //Throw an exception?
            }
        }

        /*
        public void Open(string filename)
        {
            Document file = this.gridFile.Files.FindOne(new Document().Append("filename", filename));
            if (file != null)
            {
                this.id = (Object)file["_id"];
                this.fileName = (String)file["filename"];
                this.chunkSize = (Int32)file["chunkSize"];
                this.contentType = (String)file["contentType"];
                this.length = (Int32)file["length"];
                this.aliases = file.Contains("aliases") ? (string[])file["aliases"] : null;
                this.uploadDate = (DateTime)file["uploadDate"];
                this.md5 = (string)file["md5"];
            }
            else
            {
                OidGenerator oidGenerator = new OidGenerator();
                this.id = oidGenerator.Generate();
                this.fileName = filename;
                this.chunkSize = DEFAULT_CHUNKSIZE;
                this.contentType = DEFAULT_CONTENT_TYPE;
                this.uploadDate = null;
            }
            this.isOpen = true;
        }
        
        public void Write(byte[] data)
        {
            AssertOpen();
            
        }

        public void Read(int length)
        {
            AssertOpen();
        }

        public void Flush()
        {
            AssertOpen();

        }

        public void Close(){
            AssertOpen();
            Dispose();
        }

        public bool FileExists(string filename)
        {
            ICursor results = this.gridFile.Files.Find(new Document().Append("filename", filename));
            if (results != null)
            {
                return true;
            }
            else return false;
        }

        public List<string> List(string collectionName)
        {
            List<string> names = new List<String>();
            ICursor cursor = this.gridFile.Files.FindAll();
            if (cursor == null)
            {
                //TODO: Make MongoExcption
                throw new Exception();
            }
            else
                foreach (Document file in cursor.Documents)
            {
                names.Add((string)file["filename"]);
            }
            return names;
        }

        private void AssertOpen()
        {
            if (!isOpen){
                throw new MongoGridFSException("Cannot write to a file when it is closed.", this.fileName, new Exception("file is closed"));
            }
        }

        private void FlushWriteBuffer()
        {
            List<Document> chunks = new List<Document>();
            int chunkNumber = 0;
            int offset = 0;
            int lastSize = (int)fileStream.Length % this.chunkSize;
            double nthChunk = 0;
            if (buffer.Length > this.chunkSize)
            {
                nthChunk = Math.Floor(buffer.Length / (double)this.chunkSize);
                while (offset < fileStream.Length)
                {
                    byte[] data = new byte[this.chunkSize];
                    if (chunkNumber < nthChunk)
                    {
                        data = binaryReader.ReadBytes(this.chunkSize);
                        Array.Copy(buffer, offset, data, 0, chunkSize);
                    }
                    else
                    {
                        Array.Copy(buffer, offset, data, 0, lastSize);
                    }
                    GridChunk gridChunk = new GridChunk(this.id, chunkNumber, data);
                    chunks.Add(gridChunk.ToDocument());
                    offset += this.chunkSize;
                    chunkNumber++;
                }
            }
            else  {
                GridChunk gridChunk = new GridChunk(id, 0, buffer);
                chunks.Add(gridChunk.ToDocument());
            }

            this.gridFile.Chunks.Insert(chunks);
            
            
        }
        
        public Document ToDocument(){
            Document doc = new Document();
            doc["_id"] = this.id;
            doc["filename"] = this.fileName;
            doc["contentType"] = this.contentType;
            doc["length"] = this.length;
            doc["chunkSize"] = this.chunkSize;
            if (this.uploadDate != null){
                doc["uploadDate"] = this.uploadDate;
            }
            return doc;
        }
        */
       public Document ToDocument(){
           return this.filedata;
       }
        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //TODO
                    //Dispose managed resources
                    if (fileStream != null){
                        fileStream.Dispose();
                    }
                    if (binaryReader != null){
                        binaryReader.Close();
                    }
                }
            }
            disposed = true;
            isOpen = false;
        }
        #endregion



    }
}
