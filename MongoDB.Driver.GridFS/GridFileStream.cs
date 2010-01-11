using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MongoDB.Driver.GridFS
{
    /// <summary>
    /// Stream for reading and writing to a file in GridFS.
    /// </summary>
    public class GridFileStream : Stream
    {
        private IMongoCollection chunks;
        //private List<GridChunk> chunks;
        private GridChunk chunk;
        private long chunkOffset;
        private byte[] buffer;
        private int buffWritten;
                
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
        
        public GridFileStream(GridFileInfo gridfileinfo, IMongoCollection chunks, FileAccess access){
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
            this.chunks = chunks;
            this.buffer = new byte[gridFileInfo.ChunkSize];
            //this.bufferSize = gridFileInfo.ChunkSize;
        }
        
        public override void Write(byte[] array, int offset, int count){
            ValidateWriteState(array,offset,count);
            
            //Baby steps.  First write to the first chunk.            
            EnsureWriteChunkLoaded();
            int bytesLeftToWrite = count;
            while(bytesLeftToWrite > 0){
                int buffAvailable = buffer.Length - buffWritten;
                int writeCount = 0;
                if(buffAvailable < count){
                    writeCount = buffAvailable;
                }else{
                    writeCount = count;
                }
                Array.Copy(array,offset,buffer,buffWritten,writeCount);
                buffWritten += writeCount;
                position += writeCount;
                bytesLeftToWrite -= writeCount;
                if(buffWritten >= buffer.Length){
                    FlushBufferToChunk();
                }
            }

            
//            if (buffer == null){
//                buffer = new byte[bufferSize];
//            }
//            long num = position + count;
//            if (num > bufferSize){
//                bufferSize = num;
//                byte[] buffer2 = new byte[bufferSize];
//                Array.Copy(buffer,0,buffer2,0,bufferSize);
//                buffer = buffer2;
//            }
//            Array.Copy(array, offset, this.buffer, position, count);                
//            position += count;
                                    
        }
        
        private void ValidateWriteState(byte[] array, int offset, int count){
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
        }
        
        public override long Seek(long offset, SeekOrigin origin){
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
                        position = offset;
                    break;

                case SeekOrigin.Current:
                    position += offset;
                    break;

                case SeekOrigin.End:
                    if (offset <= 0)
                    {
                        throw new ArgumentException("Attempted seeking after the end of the stream");
                    }
                    position = this.Length - offset;
                    break;                  
            }
            return position; 
        }
        
        public override void SetLength(long value){
            throw new NotImplementedException();
        }
        
        public override void Flush(){
            //Still only dealing with one chunk for now.
            FlushBufferToChunk();
            chunks.Insert(chunk.ToDocument());
        }
        
        public override int Read(byte[] array, int offset, int count){
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
//                if (buffer == null){
//                    buffer = new byte[bufferSize];
//                }                
//                Array.Copy(buffer, position, array, offset, count);
//                position += count;
//                if (count == bufferSize){
//                    return 0;
//                }
//                else
//                    return count;              
                return 0;
            }
        }
        
        private void FlushBufferToChunk(){
            //Still only dealing with one chunk for now.
            byte[] chunkBytes = new byte[buffWritten];
            chunk.Data.Bytes = chunkBytes;
            Array.Copy(buffer,0, chunkBytes,0,buffWritten);
            chunkOffset += buffWritten;
            buffWritten = 0;
        }
        
        private void EnsureWriteChunkLoaded(){
            //int chunknum = (int)Math.Floor((double)(this.position / this.gridFileInfo.ChunkSize));
            if(chunk == null){
                chunk = new GridChunk(this.GridFileInfo.Id, 0, new byte[0]);
                buffWritten = 0;
                chunkOffset = 0;
            }
        }
        
        public override void Close(){
            this.Flush();
            //Should also update gridFileInfo statistics.
            base.Close();
        }
        
        protected override void Dispose(bool disposing){
            this.canRead = false;
            this.canWrite = false;
            
            base.Dispose(disposing);
        }
    }
}
