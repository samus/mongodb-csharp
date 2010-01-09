using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MongoDB.Driver.GridFS
{
    public sealed class GridFile : IDisposable
    {
        private const int DEFAULT_CHUNKSIZE = 256 * 1024;
        private const string DEFAULT_CONTENT_TYPE = "text/plain";
        private FileMode fileMode;
        private BinaryReader binaryReader;
        private FileStream fileStream;
        private GridFS gridFS;
        private byte[] buffer;
        private bool disposed = false;
        private bool isOpen = false;

        

        public GridFile(GridFS gridFS){
            this.gridFS = gridFS;
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

        public void Open(string filename)
        {
                Document file = this.gridFS.Files.FindOne(new Document().Append("filename", filename));
                if (file != null)
                {
                    this.id = (Object)file["_id"];
                    this.filename = (String)file["filename"];
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
                    this.filename = filename;
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
            ICursor results = this.gridFS.Files.Find(new Document().Append("filename", filename));
            if (results != null)
            {
                return true;
            }
            else return false;
        }

        public List<string> List(string collectionName)
        {
            List<string> names = new List<String>();
            ICursor cursor = this.gridFS.Files.FindAll();
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
                throw new MongoGridFSException("Cannot write to a file when it is closed.", this.filename, new Exception("file is closed"));
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

            this.gridFS.Chunks.Insert(chunks);                
        }


        private Object id;
        public Object Id{
            get { return this.id; }
            set { this.id = value; }
        }

        private string filename;
        public string Filename
        {
            get { return this.filename; }
            set { this.filename = value; }
        }

        private string contentType;
        public string ContentType{
            get { return this.contentType; }
            set { this.contentType = value; }
        }

        private int length;
        public int Length{
            get { return this.length; }
            set { this.length = value; }
        }

        private string[] aliases;
        public string[] Aliases{
            get { return this.aliases; }
            set { this.aliases = value; }
        }

        private int chunkSize;
        public int ChunkSize{
            get { return this.chunkSize; }
            set { this.chunkSize = value; }
        }

        private Object metadata;
        public Object Metadata{
            get { return this.metadata; }
        }

        private DateTime? uploadDate;
        public DateTime? UploadDate{
            get { return this.uploadDate; }
            set { this.uploadDate = value; }
        }
        private string md5;
        public string Md5{
            get { return this.md5; }
            set { this.md5 = value; }
        }

        
        public Document ToDocument(){
            Document doc = new Document();
            doc["_id"] = this.id;
            doc["filename"] = this.filename;
            doc["contentType"] = this.contentType;
            doc["length"] = this.length;
            doc["chunkSize"] = this.chunkSize;
            if (this.uploadDate != null){
                doc["uploadDate"] = this.uploadDate;
            }            
            return doc;
        }




    }
}
