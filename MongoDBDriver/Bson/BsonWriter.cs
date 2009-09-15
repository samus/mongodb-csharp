
using System;
using System.IO;
using System.Text;

namespace MongoDB.Driver.Bson
{
    
    /// <summary>
    /// Writes primitives to a stream.  Use Documents and Elements to write more complex items.
    /// </summary>
    public class BsonWriter :  IDisposable
    {
        private Stream stream;
        private BinaryWriter writer;
        private UTF8Encoding encoding = new UTF8Encoding();
        private int bytesWritten = 0;
        
        public BsonWriter(Stream stream){
            this.stream = stream;
            writer = new BinaryWriter(this.stream);
        }
        
        /// <summary>
        /// Writes a CString to the stream followed by a null terminator
        /// </summary>
        /// <param name="str">
        /// A <see cref="System.String"/>
        /// </param>
        public void Write(String str){
            Byte[] buf = new byte[encoding.GetByteCount(str) + 1];
            buf[buf.Length - 1] = (byte)0;
            encoding.GetBytes(str,0,str.Length,buf,0);
            writer.Write(buf);
            bytesWritten += buf.Length;
        }
        
         public void Write(Boolean val){
            if(val){
                this.Write((Byte)1);
            }else{
                this.Write((Byte)0);
            }
            bytesWritten += 1;
        }
        
        public void Write(Byte val){
            writer.Write(val);
            bytesWritten += 1;
        }
        
        public void Write(Byte[] val){
            writer.Write(val);
            bytesWritten += val.Length;
        }
        
        public void Write(Int32 val){
            writer.Write(val);
            bytesWritten += 4;
        }
        
        public void Write(Int64 val){
            writer.Write(val);
            bytesWritten += 8;
        }
        
        public void Write(double val){
            writer.Write(val);
            bytesWritten += sizeof(double);
        }
        
        public void Flush(){
            writer.Flush();
        }
        
        public void Dispose(){
            return;
        }
        
    }
}
