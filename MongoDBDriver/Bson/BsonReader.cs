
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MongoDB.Driver.Bson
{
    
    
    public class BsonReader
    {
        private Stream stream;
        private BinaryReader reader;
        private UTF8Encoding encoding = new UTF8Encoding();
        
        
        public BsonReader(Stream stream){
            this.stream = stream;
            reader = new BinaryReader(this.stream);
        }
        
        /// <summary>
        /// Reads a string from the stream.  The length should include the null terminator in the size.
        /// </summary>
        /// <param name="length">How much to read including null terminator</param>
        /// <returns></returns>
        public String ReadString(int length){
            byte[] buff = reader.ReadBytes(length -1);
            reader.ReadByte(); //ignore the terminator.
            string ret = encoding.GetString(buff);
            return ret;
        }
        
        /// <summary>
        /// Reads from the stream one byte at a time until an ascii null (0) is found and builds a string from the bytes.
        /// Use ReadString(length) when the length is known ahead of time as it is more efficient.
        /// </summary>
        /// <returns></returns>
        public String ReadString(){
            List<byte> buff = new List<byte>();
            byte b = reader.ReadByte();
            while(b != 0){
                buff.Add(b);
                b = reader.ReadByte();
            }
            string ret = encoding.GetString(buff.ToArray());
            return ret;         
        }
        
        public bool ReadBoolean(){
            byte val = reader.ReadByte();
            if(val == 0){
                return false;
            }else{
                return true;
            }
        }

        public byte ReadByte(){
            return reader.ReadByte();
        }
        public byte[] ReadBytes(int len){
            return reader.ReadBytes(len);
        }       
        
        public Int32 ReadInt32(){
            return reader.ReadInt32();
        }
        
        public Int64 ReadInt64(){
            return reader.ReadInt64();
        }
        
        public double ReadDouble(){
            return reader.ReadDouble();
        }       
    }
}
