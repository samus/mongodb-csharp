using System;
using System.IO;

using MongoDB;

namespace MongoDB.GridFS
{
    /// <summary>
    /// Stream for reading and writing to a file in GridFS.
    /// </summary>
    /// <remarks>
    /// When using the stream for random io it is possible to produce chunks in the begining and middle of the
    /// file that are not full size followed by other chunks that are full size.  This only affects the md5 sum
    /// that is calculated on the file on close.  Because of this do not rely on the md5 sum of a file when doing
    /// random io.  Writing to the stream sequentially works fine and will produce a consistent md5.
    /// </remarks>
    public class GridFileStream : Stream
    {

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
        private long highestPosWritten;


        #region Properties
        private GridFileInfo gridFileInfo;
        /// <summary>
        /// Gets or sets the grid file info.
        /// </summary>
        /// <value>The grid file info.</value>
        public GridFileInfo GridFileInfo {
            get { return gridFileInfo; }
            set { gridFileInfo = value; }
        }

        private bool canRead;
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports reading; otherwise, false.
        /// </returns>
        public override bool CanRead {
            get { return canRead; }
        }

        private bool canWrite;
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports writing; otherwise, false.
        /// </returns>
        public override bool CanWrite {
            get { return canRead; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports seeking; otherwise, false.
        /// </returns>
        public override bool CanSeek {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// A class derived from Stream does not support seeking.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Length {
            get { return gridFileInfo.Length; }
        }

        private long position;
        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Position {
            get { return position; }
            set { this.Seek (value, SeekOrigin.Begin); }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GridFileStream"/> class.
        /// </summary>
        /// <param name="gridfileinfo">The gridfileinfo.</param>
        /// <param name="files">The files.</param>
        /// <param name="chunks">The chunks.</param>
        /// <param name="access">The access.</param>
        public GridFileStream (GridFileInfo gridfileinfo, IMongoCollection files, 
                               IMongoCollection chunks, FileAccess access)
        {
            switch (access) {
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
            this.highestPosWritten = this.gridFileInfo.Length;
            this.MoveTo (0);
        }

        /// <summary>
        /// Reads data from the stream into the specified array.  It will fill the array in starting at offset and
        /// adding count bytes returning the number of bytes read from the stream.
        /// </summary>
        public override int Read (byte[] array, int offset, int count)
        {
            ValidateReadState (array, offset, count);
            
            int bytesLeftToRead = count;
            int bytesRead = 0;
            while (bytesLeftToRead > 0 && this.position < this.Length) {
                int buffAvailable = buffer.Length - buffPosition;
                int readCount = 0;
                if (buffAvailable > bytesLeftToRead) {
                    readCount = bytesLeftToRead;
                } else {
                    readCount = buffAvailable;
                }
                if (readCount + position > highestPosWritten) {
                    //adjust readcount so that we don't read past the end of file.
                    readCount = readCount - (int)(readCount + position - highestPosWritten);
                }
                Array.Copy (buffer, buffPosition, array, offset, readCount);
                buffPosition += readCount;
                bytesLeftToRead -= readCount;
                bytesRead += readCount;
                offset += readCount;
                MoveTo (position + readCount);
            }
            return bytesRead;
        }

        private void ValidateReadState (byte[] array, int offset, int count)
        {
            if (array == null) {
                throw new ArgumentNullException ("array", new Exception ("array is null"));
            } else if (offset < 0) {
                throw new ArgumentOutOfRangeException ("offset", new Exception ("offset is negative"));
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException ("count", new Exception ("count is negative"));
            } else if ((array.Length - offset) < count) {
                throw new MongoGridFSException ("Invalid count argument", gridFileInfo.FileName, null);
            } else if (!canRead) {
                throw new MongoGridFSException ("Reading this file is not supported", gridFileInfo.FileName, null);
            }
        }

        /// <summary>
        /// Copies from the source array into the grid file.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">A <see cref="System.Int32"/>  The offset within the source array.</param>
        /// <param name="count">A <see cref="System.Int32"/>  The number of bytes from within the source array to copy.</param>
        public override void Write (byte[] array, int offset, int count)
        {
            ValidateWriteState (array, offset, count);
            
            int bytesLeftToWrite = count;
            while (bytesLeftToWrite > 0) {
                int buffAvailable = buffer.Length - buffPosition;
                int writeCount = 0;
                if (buffAvailable > bytesLeftToWrite) {
                    writeCount = bytesLeftToWrite;
                } else {
                    writeCount = buffAvailable;
                }
                Array.Copy (array, offset, buffer, buffPosition, writeCount);
                chunkDirty = true;
                buffPosition += writeCount;
                offset += writeCount;
                bytesLeftToWrite -= writeCount;
                MoveTo (position + writeCount);
                highestPosWritten = Math.Max (highestPosWritten, position);
            }
        }

        private void ValidateWriteState (byte[] array, int offset, int count)
        {
            if (array == null) {
                throw new ArgumentNullException ("array", new Exception ("array is null"));
            } else if (offset < 0) {
                throw new ArgumentOutOfRangeException ("offset", new Exception ("offset is negative"));
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException ("count", new Exception ("count is negative"));
            } else if ((array.Length - offset) < count) {
                throw new MongoGridFSException ("Invalid count argument", gridFileInfo.FileName, null);
            } else if (!canWrite) {
                throw new System.NotSupportedException ("Stream does not support writing.");
            }
        }


        /// <summary>
        /// Flushes any changes to current chunk to the database.  It can be called in client code at any time or it
        /// will automatically be called on Close() and when the stream position moves off the bounds of the current
        /// chunk.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override void Flush ()
        {
            if (chunkDirty == false)
                return;
            //avoid a copy if possible.
            if (highestBuffPosition == buffer.Length) {
                chunk["data"] = new Binary (buffer);
            } else {
                byte[] data = new byte[highestBuffPosition];
                Array.Copy (buffer, data, highestBuffPosition);
                chunk["data"] = new Binary (data);
            }
            
            
            if (chunk.Contains ("_id")) {
                chunks.Save (chunk);
            } else {
                chunks.Insert (chunk);
            }
            this.gridFileInfo.Length = highestPosWritten;
        }

        /// <summary>
        /// Seek to any location in the stream.  Seeking past the end of the file is allowed.  Any writes to that
        /// location will cause the file to grow to that size.  Any holes that may be created from the seek will
        /// be zero filled on close.
        /// </summary>
        public override long Seek (long offset, SeekOrigin origin)
        {
            if ((origin < SeekOrigin.Begin) || (origin > SeekOrigin.End)) {
                throw new ArgumentException ("Invalid Seek Origin");
            }
            
            switch (origin) {
            case SeekOrigin.Begin:
                if (offset < 0) {
                    throw new ArgumentException ("Attempted seeking before the begining of the stream");
                } else {
                    MoveTo (offset);
                }
                break;
            case SeekOrigin.Current:
                MoveTo (position + offset);
                break;
            case SeekOrigin.End:
                if (offset <= 0) {
                    throw new ArgumentException ("Attempted seeking after the end of the stream");
                }
                MoveTo (this.Length - offset);
                break;
            }
            return position;
        }

        /// <summary>
        /// Sets the length of this stream to the given value.
        /// </summary>
        /// <param name="value">
        /// A <see cref="System.Int64"/>
        /// </param>
        public override void SetLength (long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException ("length");
            if (this.CanSeek == false || this.CanWrite == false) {
                throw new NotSupportedException ("The stream does not support both writing and seeking.");
            }
            
            if (value < highestPosWritten) {
                TruncateAfter (value);
            } else {
                this.Seek (value, SeekOrigin.Begin);
            }
            chunkDirty = true;
            this.gridFileInfo.Length = value;
            highestPosWritten = value;
            
        }

        /// <summary>
        /// Close the stream and flush any changes to the database.
        /// </summary>
        public override void Close ()
        {
            this.Flush ();
            this.gridFileInfo.Length = highestPosWritten;
            EnsureNoHoles ();
            string md5 = gridFileInfo.CalcMD5 ();
            gridFileInfo.Md5 = md5;
            this.files.Save (gridFileInfo.ToDocument ());
            base.Close ();
        }

        /// <summary>
        /// Moves the current position to the new position.  If this causes a new chunk to need to be loaded it will take
        /// care of flushing the buffer and loading a new chunk.
        /// </summary>
        /// <param name="position">
        /// A <see cref="System.Int32"/> designating where to go to.
        /// </param>
        private void MoveTo (long position)
        {
            this.position = position;
            int chunkSize = this.gridFileInfo.ChunkSize;
            bool chunkInRange = (chunk != null && position >= chunkLower && position < chunkUpper);
            if (chunkInRange == false) {
                if (chunk != null && chunkDirty) {
                    highestBuffPosition = Math.Max (highestBuffPosition, buffPosition);
                    this.Flush ();
                }
                int chunknum = (int)Math.Floor ((double)(position / chunkSize));
                Array.Copy (blankBuffer, buffer, buffer.Length);
                LoadOrCreateChunk (chunknum);
                chunkDirty = false;
                chunkLower = chunknum * chunkSize;
                chunkUpper = chunkLower + chunkSize;
            }
            buffPosition = (int)(position % chunkSize);
            highestBuffPosition = Math.Max (highestBuffPosition, buffPosition);
            
        }

        /// <summary>
        /// Loads a chunk from the chunks collection if it exists.  Otherwise it creates a blank chunk Document.
        /// </summary>
        /// <param name="num"></param>
        private void LoadOrCreateChunk (int num)
        {
            Object fid = this.GridFileInfo.Id;
            Document spec = new Document().Add("files_id", fid).Add("n", num);
            chunk = this.chunks.FindOne (spec);
            if (chunk == null) {
                chunk = spec;
                highestBuffPosition = 0;
            } else {
                Binary b = (Binary)chunk["data"];
                highestBuffPosition = b.Bytes.Length;
                Array.Copy (b.Bytes, buffer, highestBuffPosition);
            }
        }


        /// <summary>
        /// Deletes all chunks after the specified position and clears out any extra bytes if the position doesn't fall on
        /// a chunk boundry.
        /// </summary>
        private void TruncateAfter (long value)
        {
            int chunknum = CalcChunkNum (value);
            Document spec = new Document().Add("files_id", this.gridFileInfo.Id).Add("n", new Document().Add("$gt", chunknum));
            this.chunks.Delete (spec);
            this.MoveTo (value);
            Array.Copy (blankBuffer, 0, buffer, buffPosition, buffer.Length - buffPosition);
            highestBuffPosition = buffPosition;
        }

        private int CalcChunkNum (long position)
        {
            int chunkSize = this.gridFileInfo.ChunkSize;
            return (int)Math.Floor ((double)(position / chunkSize));
        }

        /// <summary>
        /// Makes sure that at least a skelton chunk exists for all numbers.  If not the MD5 calculation will fail on a sparse file.
        /// </summary>
        private void EnsureNoHoles ()
        {
            int highChunk = CalcChunkNum (this.GridFileInfo.Length);
            Document query = new Document().Add("files_id", this.GridFileInfo.Id).Add("n", new Document().Add("$lte", highChunk));
            Document sort = new Document().Add("n", 1);
            Document fields = new Document().Add("_id", 1).Add("n", 1);
            
            Binary data = new Binary (this.blankBuffer);
            int i = 0;
            using(ICursor cur = chunks.Find(new Document().Add("query", query).Add("sort", sort), 0, 0, fields)){
                foreach (Document doc in cur.Documents) {
                    int n = Convert.ToInt32 (doc["n"]);
                    if (i < n) {
                        while (i < n) {
                            chunks.Insert (new Document ().Add("files_id", this.gridFileInfo.Id).Add("n", i).Add("data", data));
                            i++;
                        }
                    } else {
                        i++;
                    }
                }
            }
            
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.Stream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose (bool disposing)
        {
            this.canRead = false;
            this.canWrite = false;
            
            base.Dispose (disposing);
        }
    }
}
