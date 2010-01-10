
using System;
using System.IO;

namespace MongoDB.Driver.GridFS
{
    /// <summary>
    /// Stream for reading and writing to a file in GridFS.
    /// </summary>
    public class GridFileStream : Stream
    {
        private List<GridChunk> chunks;
        private byte[] buffer;
        private int readPos;
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
            else if (offset < 0){
                throw new ArgumentOutOfRangeException("offset", new Exception("offset is negative"));
            }
            else if (count < 0){
                throw new ArgumentOutOfRangeException("count",new Exception("count is negative"));
            }
            else if ((array.Length - offset) < count){
                throw new MongoGridFSException("Invalid count argument", gridFileInfo.FileName, null);
            }
            else if (!canWrite){
                throw new MongoGridFSException("Writing to this file is not supported", gridFileInfo.FileName, null);
            }
            else{
                if (buffer == null){
                    buffer = new byte[bufferSize];
                }
                int num = writePos + count;
                if (num > bufferSize){
                    bufferSize = num;
                    byte[] buffer2 = new byte[bufferSize];
                    Array.Copy(buffer,0,buffer2,0,bufferSize);
                    buffer = buffer2;
                }
                Array.Copy(array, offset, this.buffer, writePos, count);                
                this.writePos += count;
            }            
        }
        
        public override int Seek(int offset, SeekOrigin origin){
            if ((origin < SeekOrigin.Begin) || (origin > SeekOrigin.End))
            {
                throw new ArgumentException("Invalid Seek Origin");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0){
                        throw new ArgumentException("Attempted seeking before the begining of the stream");
                    }
                    else
                        this.readPos = offset;
                    break;

                case SeekOrigin.Current:
                    readPos += offset;
                    break;

                case SeekOrigin.End:
                    if (offset <= 0)
                    {
                        throw new ArgumentException("Attempted seeking after the end of the stream");
                    }
                    else this.readPos = bufferSize + offset;
                    break;                  
            }
            return readPos;
 
        }
        
        public override void SetLength(long value){
            throw new NotImplementedException();
        }
        
        public override void Flush(){
            throw new NotImplementedException();
        }
        
        public override int Read([In,Out]byte[] array, int offset, int count){
            if (array == null)            {
                throw new ArgumentNullException("array", new Exception("array is null"));
            }
            else if (offset < 0){
                throw new ArgumentOutOfRangeException("offset", new Exception("offset is negative"));
            }
            else if (count < 0){
                throw new ArgumentOutOfRangeException("count", new Exception("count is negative"));
            }
            else if ((array.Length - offset) < count){
                throw new MongoGridFSException("Invalid count argument", gridFileInfo.FileName, null);
            }
            else if (!canRead){
                throw new MongoGridFSException("Reading this file is not supported", gridFileInfo.FileName, null);
            }
            else
            {
                if (buffer == null){
                    buffer = new byte[bufferSize];
                }                
                Array.Copy(buffer, readPos, array, offset, count);
                readPos += count;
                if (count == bufferSize){
                    return 0;
                }
                else
                    return count;              
            }
        }


        protected override void Dispose(bool disposing){
            this.canRead = false;
            this.canWrite = false;
            base.Dispose(disposing);
        }
    }
}
