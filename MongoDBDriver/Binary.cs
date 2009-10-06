using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver
{
	public class Binary
	{
        private byte[] bytes;        
        public byte[] Bytes{
            get { return this.bytes; }
            set { this.bytes = value; }
            
        }
 
        private byte subtype;
        public byte Subtype{
            get { return this.subtype; }
            set { this.subtype = value; }
        }

        public Binary() { }

        public Binary(byte[] value){
            this.Bytes = value;
            this.Subtype = (byte)2;          
        }

      

    }
}
