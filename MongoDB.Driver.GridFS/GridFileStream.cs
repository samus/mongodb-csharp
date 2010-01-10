
using System;
using System.IO;

namespace MongoDB.Driver.GridFS
{
    /// <summary>
    /// Stream for reading and writing to a file in GridFS.
    /// </summary>
    public class GridFileStream : Stream
    {
        private GridChunk chunk;
        private byte[] buffer;
        private int readPos;
        private int readLen;
        private int writePos;
        private int bufferSize;
                
        #region Properties
        private GridFileInfo gridFileInfo;       
        public GridFileInfo GridFileInfo {
            get { return gridFileInfo; }
            set { gridFileInfo = value; }
        }
                
        private bool canRead;
        public override bool CanRead {
            get { return canRead; }
        }
        
        private bool canWrite;
        public override bool CanWrite {
            get { return canRead; }
        }
        
        public override bool CanSeek {
            get { return true; }
        }
        
        public override long Length {
            get {
                return gridFileInfo.Length;
            }
        }

        private long position;
        public override long Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }
        #endregion
        
        public GridFileStream(GridFileInfo gridfileinfo, FileAccess access){
            switch (access){
                case FileAccess.Read:
                    canRead = true;
                    break;
                case FileAccess.ReadWrite:
                    canRead = true;
                    canWrite = true;
                    break;
                case FileAccess.Write:
                    canWrite = true;
                break;
            }
            this.gridFileInfo = gridfileinfo;
            this.bufferSize = gridFileInfo.ChunkSize;
        }
        
        public override void Write(byte[] array, int offset, int count){
            if (array == null){
                throw new ArgumentNullException("array", new Exception("array is null"));
            }
            if (offset < 0){
                throw new ArgumentOutOfRangeException("offset", new Exception("offset is negative"));
            }
            if (count < 0){
                throw new ArgumentOutOfRangeException("count",new Exception("count is negative"));
            }
            if ((array.Length - offset) < count){
                throw new MongoGridFSException("Invalid count argument", gridFileInfo.FileName, null);
            }
            if (!canWrite){
                throw new MongoGridFSException("Writing to this file is not supported", gridFileInfo.FileName, null);
            }
            else{
                int num = writePos + count;
                if (num > bufferSize){
                    bufferSize = num;
                    byte[] buffer2 = new byte[bufferSize];
                    Buffer.BlockCopy(buffer,0,buffer2,0,bufferSize);
                    buffer = buffer2;
                }
                Buffer.BlockCopy(array, offset, this.buffer, writePos, count);
                this.writePos += count;
            }            
        }
        
        public override long Seek(long offset, SeekOrigin origin){
            throw new NotImplementedException();
        }
        
        public override void SetLength(long value){
            throw new NotImplementedException();
        }
        
        public override void Flush(){
            throw new NotImplementedException();
        }
        
        public override int Read(byte[] buffer, int offset, int count){
            throw new NotImplementedException();
        }


        protected override void Dispose(bool disposing){
            this.canRead = false;
            this.canWrite = false;
            base.Dispose(disposing);
        }
    }
}
