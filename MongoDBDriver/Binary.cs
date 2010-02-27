using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Driver
{
	public class Binary{
		public enum TypeCode:byte{
			Unknown = 0,
			General = 2,

            // Uuid is now replaced by Binary
			//Uuid = 3,
			
            Md5 = 5,
			UserDefined = 80
		}
			
        private byte[] bytes;        
        public byte[] Bytes{
            get { return this.bytes; }
            set { this.bytes = value; }
            
        }
 
        private Binary.TypeCode subtype;
        public Binary.TypeCode Subtype{
            get { return this.subtype; }
            set { this.subtype = value; }
        }

        public Binary() { }

        public Binary(byte[] value){
            this.Bytes = value;
            this.Subtype = TypeCode.General;         
        }

      

    }
}
