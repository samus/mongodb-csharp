using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using MongoDB.Driver;

namespace MongoDB.Driver.GridFS
{
    /// <summary>
    /// Stream for reading and writing to a file in GridFS.
    /// </summary>
    public class GridFileStream : Stream
    {
        private IMongoCollection files;
        private IMongoCollection chunks;
        private Document chunk;
        private bool chunkDirty;
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
        
        public GridFileStream(GridFileInfo gridfileinfo,IMongoCollection files, IMongoCollection chunks, FileAccess access){
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
            this.files = files;
            this.chunks = chunks;
            this.buffer = new byte[gridFileInfo.ChunkSize];
        }
        
        /// <summary>
        /// Copies from the source array into the grid file.
        /// </summary>
        /// <param name="array">
        /// A <see cref="System.Byte[]"/>  The source array to copy from.
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>  The offset within the source array.
        /// </param>
        /// <param name="count">
        /// A <see cref="System.Int32"/>  The number of bytes from within the source array to copy.
        /// </param>
        public override void Write(byte[] array, int offset, int count){
            ValidateWriteState(array,offset,count);
            
            //Baby steps.  First write to the first chunk.            
            int bytesLeftToWrite = count;
            while(bytesLeftToWrite > 0){
                EnsureWriteChunkLoaded();
                int buffAvailable = buffer.Length - buffWritten;
                int writeCount = 0;
                if(buffAvailable > bytesLeftToWrite){
                    writeCount = bytesLeftToWrite;
                }else{
                    writeCount = buffAvailable;
                }
                Array.Copy(array,offset,buffer,buffWritten,writeCount);
                chunkDirty = true;
                buffWritten += writeCount;
                position += writeCount;
                bytesLeftToWrite -= writeCount;
                if(buffWritten >= buffer.Length){
                    Flush();
                }
            }
        }

        private void EnsureWriteChunkLoaded(){
            int chunknum = (int)Math.Floor((double)(this.position / this.gridFileInfo.ChunkSize));
            if(chunk == null){
                chunk = new Document().Append("files_id", this.GridFileInfo.Id).Append("n",chunknum);
                buffWritten = 0;
                chunkDirty = false;
            }
        }
        
        public override void Flush(){
            if(chunkDirty == false) return;
            byte[] data = new byte[buffWritten];
            Array.Copy(buffer,data,buffWritten);
            
            chunk["data"] = new Binary(data);
            
            if(chunk.Contains("_id")){
                chunks.Update(chunk);
            }else{
                chunks.Insert(chunk);
            }
            if(buffWritten >= this.GridFileInfo.ChunkSize){
                chunk = null;
                EnsureWriteChunkLoaded();
            }
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

        public override int Read(byte[] array, int offset, int count){

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

        private void ValidateReadState(byte[] array, int offset, int count){
            if (array == null){
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
        }
        
        public override void Close(){
            this.Flush();
            this.files.Update(this.GridFileInfo.ToDocument());
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
