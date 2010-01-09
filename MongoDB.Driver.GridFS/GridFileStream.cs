
using System;
using System.IO;

namespace MongoDB.Driver.GridFS
{
    /// <summary>
    /// Stream for reading and writing to a file in GridFS.
    /// </summary>
    public class GridFileStream : Stream
    {
        private GridFileInfo gridFileInfo;
        
        #region Properties
        public override bool CanRead {
            get { return true; }
        }
        public override bool CanWrite {
            get { return true; }
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
        
        public GridFileStream(GridFileInfo gridfileinfo){
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
