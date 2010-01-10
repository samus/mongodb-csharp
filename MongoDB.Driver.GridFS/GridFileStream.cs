
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
            this.gridFileInfo = gridFileInfo;
        }
        
        public override void Write(byte[] buffer, int offset, int count){
            throw new NotImplementedException();
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
    }
}
