using System;
using System.Text.RegularExpressions;

namespace MongoDB.Driver{

    public class Oid
    {
        private byte[] value;
        public byte[] Value {
            get { return this.value; }
            set {
                this.value = value; 
            }
        }

        public DateTime Created{
            get{
                byte[] time = new byte[4];
                Array.Copy(this.value,time,4);
                Array.Reverse(time);
                int seconds = BitConverter.ToInt32(time,0);
                return OidGenerator.epoch.AddSeconds(seconds);

            }
        }
        public Oid(){}
        
        public Oid(string value){
            ValidateHex(value);
            this.Value = DecodeHex(value);
        }
        
        public Oid(byte[] value){
            this.Value = value;
        }
        
        public override bool Equals(object obj){
            if(obj.GetType() == typeof(Oid)){
                string hex = ((Oid)obj).ToString();
                return this.ToString().Equals(hex);
            }
            return false;
        }
        
        public override string ToString() {
            return string.Format(@"ObjectId(""{0}"")", BitConverter.ToString(value).Replace("-","").ToLower());
        }
        
        protected void ValidateHex(string val){
            if(val == null || val.Length != 24) throw new ArgumentException("Oid strings should be 24 characters");
            
            Regex notHexChars = new Regex(@"[^A-Fa-f0-9]", RegexOptions.None);
            if(notHexChars.IsMatch(val)){
                throw new ArgumentOutOfRangeException("val","Value contains invalid characters");
            }
        }
        
        protected static byte[] DecodeHex(string val){
            int numberChars = val.Length;

            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2){
                try{
                    bytes[i / 2] = Convert.ToByte(val.Substring(i, 2), 16);
                }
                catch{
                    //failed to convert these 2 chars, they may contain illegal charracters
                    bytes[i / 2] = 0;
                }
            }
            return bytes;            
        }
    }
}
