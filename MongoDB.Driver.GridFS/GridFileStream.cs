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
        private int chunkOffset;
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
                    //?Flush();
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

        private void EnsureWriteChunkLoaded(){
            //int chunknum = (int)Math.Floor((double)(this.position / this.gridFileInfo.ChunkSize));
            if(chunk == null){
                chunk = new GridChunk(this.GridFileInfo.Id, 0, new byte[0]);
                buffWritten = 0;
                chunkOffset = 0;
            }
        }

        /// <summary>
        /// Flushes the internal buffer to one or more chunks.
        /// </summary>
        private void FlushBufferToChunk(){
            //There are several ways that this method can be called.
            //It could be called as part of Flush in which case the
            //chunk size may not be the full size.
            //It could be called on a chunk that has been persisted already
            //If it isn't at the full size yet, the array should be resized
            //up to the total chunk size and as much as the buffer copied to
            //it as possible.  If there are left over bytes then we need to create
            //a new chunk and write to that.

            int bytesToWrite = buffWritten;
            int bufferOffset = 0;
            byte[] chunkBytes = chunk.Data.Bytes;
            while(bytesToWrite > 0){
                int chunkAvailable = chunkBytes.Length - chunkOffset;
                int writeCount = 0;
                if(chunkAvailable < bytesToWrite){
                    //need to resize chunk if possible.
                    writeCount = ResizeChunk(bytesToWrite - chunkAvailable) - chunkOffset;
                    chunkBytes = chunk.Data.Bytes;
                }else{
                    writeCount = buffWritten;
                }

                Console.WriteLine(string.Format("buffer.length {0}, buffOffset {1}, " +
                                                "chunkBytes.Length {2},chunkOffset {3}, writeCount {4}",
                                                buffer.Length, bufferOffset, chunkBytes.Length, chunkOffset, writeCount));
                Array.Copy(buffer,bufferOffset,chunkBytes,chunkOffset,writeCount);
                bytesToWrite -= writeCount;
                bufferOffset += writeCount;
                chunkOffset += writeCount;
                if(chunkOffset >= this.GridFileInfo.ChunkSize){
                    Console.WriteLine("Saving chunk");
                    SaveChunk();
                    chunk = new GridChunk(this.GridFileInfo.Id, chunk.N + 1, new byte[0]);
                    chunkOffset = 0;
                }
            }
            buffWritten = 0;
        }

        /// <summary>
        /// Resizes the chunk adding as much of the requested additional length and returns the new chunk size.
        /// </summary>
        private int ResizeChunk(int additionalLength){
            Byte[] bytes = chunk.Data.Bytes;
            int newsize = 0;
            int maxchunkSize = this.GridFileInfo.ChunkSize;
            if(bytes.Length + additionalLength <= maxchunkSize){
                newsize = bytes.Length + additionalLength;
            }else{
                newsize = maxchunkSize;
            }
            Array.Resize(ref bytes, newsize);
            chunk.Data.Bytes = bytes;
            return newsize;
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
            SaveChunk();
        }
        private void SaveChunk(){
            Document cd = chunk.ToDocument();
            if(cd.Contains("_id")){
                chunks.Update(cd);
            }else{
                chunks.Insert(cd);
                chunk.Id = cd["_id"];
            }
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
