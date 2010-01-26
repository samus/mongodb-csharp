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
//        public static String CHUNKID = "_id";
//        public static String CHUNKFILESID = "files_id";
//        public static String CHUNKN = "n";
//        public static String CHUNKDATA = "data";
        
        private IMongoCollection files;
        private IMongoCollection chunks;
        private Document chunk;
        private bool chunkDirty;
        private long chunkLower = -1;
        private long chunkUpper = -1;

        private byte[] buffer;
        private byte[] blankBuffer;
        private int buffPosition;
        private int highestBuffPosition;
                
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
                //FIXME This won't be the right value when the file has been written to but not saved.
                return gridFileInfo.Length;
            }
        }

        private long position;
        public override long Position {
            get {
                return position;
            }
            set {
                this.Seek(value, SeekOrigin.Begin);
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
            this.blankBuffer = new byte[gridFileInfo.ChunkSize];
            this.MoveTo(0);
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
                     
            int bytesLeftToWrite = count;
            while(bytesLeftToWrite > 0){
                int buffAvailable = buffer.Length - buffPosition;
                int writeCount = 0;
                if(buffAvailable > bytesLeftToWrite){
                    writeCount = bytesLeftToWrite;
                }else{
                    writeCount = buffAvailable;
                }
                Array.Copy(array,offset,buffer,buffPosition,writeCount);
                chunkDirty = true;
                buffPosition += writeCount;
                bytesLeftToWrite -= writeCount;
                MoveTo(position + writeCount);
            }
        }
        
        public override void Flush(){
            if(chunkDirty == false) return;
            byte[] data = new byte[highestBuffPosition];
            Array.Copy(buffer,data,highestBuffPosition);
            
            chunk["data"] = new Binary(data);
            
            if(chunk.Contains("_id")){
                chunks.Update(chunk);
            }else{
                chunks.Insert(chunk);
            }
        }
        
        private void ValidateWriteState(byte[] array, int offset, int count){
            if (array == null){
                throw new ArgumentNullException("array", new Exception("array is null"));
            }else if (offset < 0){
                throw new ArgumentOutOfRangeException("offset", new Exception("offset is negative"));
            }else if (count < 0){
                throw new ArgumentOutOfRangeException("count",new Exception("count is negative"));
            }else if ((array.Length - offset) < count){
                throw new MongoGridFSException("Invalid count argument", gridFileInfo.FileName, null);
            }else if (!canWrite){
                throw new MongoGridFSException("Writing to this file is not allowed", gridFileInfo.FileName, null);
            }
        }
        
        public override long Seek(long offset, SeekOrigin origin){
            if ((origin < SeekOrigin.Begin) || (origin > SeekOrigin.End)){
                throw new ArgumentException("Invalid Seek Origin");
            }
            
            switch (origin){
                case SeekOrigin.Begin:
                    if (offset < 0){
                        throw new ArgumentException("Attempted seeking before the begining of the stream");
                    }else{
                        MoveTo(offset);
                    }
                    break;
                case SeekOrigin.Current:
                    MoveTo(position + offset);
                    break;
                case SeekOrigin.End:
                    if (offset <= 0){
                        throw new ArgumentException("Attempted seeking after the end of the stream");
                    }
                    MoveTo(this.Length - offset);
                    break;                  
            }
            return position; 
        }


        /// <summary>
        /// Moves the current position to the new position.  If this causes a new chunk to need to be loaded it will take
        /// care of flushing the buffer and loading a new chunk. 
        /// </summary>
        /// <param name="position">
        /// A <see cref="System.Int32"/> designating where to go to.
        /// </param>
        private void MoveTo(long position){
            this.position = position;
            int chunkSize = this.gridFileInfo.ChunkSize;
            bool chunkInRange = (chunk != null && position >= chunkLower && position < chunkUpper);
            if(chunkInRange == false){
            	if(chunk != null  && chunkDirty){
            		highestBuffPosition = Math.Max(highestBuffPosition, buffPosition);
            		this.Flush();
            	}
                int chunknum = (int)Math.Floor((double)(position / chunkSize));
                Array.Copy(blankBuffer,buffer,buffer.Length);
                LoadOrCreateChunk(chunknum);
                chunkDirty = false;
                chunkLower = chunknum * chunkSize;
                chunkUpper = chunkLower + chunkSize;
            }
            buffPosition = (int)(position % chunkSize);
        	highestBuffPosition = Math.Max(highestBuffPosition, buffPosition);
            
        }

        /// <summary>
        /// Loads a chunk from the chunks collection if it exists.  Otherwise it creates a blank chunk Document.
        /// </summary>
        /// <param name="num"></param>
        private void LoadOrCreateChunk(int num){
        	Object fid = this.GridFileInfo.Id;
        	Document spec = new Document().Append("files_id", fid).Append("n",num);
        	chunk = this.chunks.FindOne(spec);
        	if(chunk == null) {
        		chunk = spec;
        		highestBuffPosition = 0;
        	}else{
        		Binary b = (Binary)chunk["data"];
        		highestBuffPosition = b.Bytes.Length;
        		Array.Copy(b.Bytes,buffer, highestBuffPosition);
        	}
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
