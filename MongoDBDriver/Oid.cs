using System;
using System.Text.RegularExpressions;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver{

    /// <summary>
    /// Oid is an immutable object that represents a Mongo ObjectId.
    /// </summary>
    public class Oid: IEquatable<Oid>, IComparable<Oid>
    {
        private static readonly OidGenerator oidGenerator = new OidGenerator();
        
        private byte[] bytes;

        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created{
            get{
                byte[] time = new byte[4];
                Array.Copy(bytes,time,4);
                Array.Reverse(time);
                int seconds = BitConverter.ToInt32(time,0);
                return BsonInfo.Epoch.AddSeconds(seconds);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Oid(string value){
            value = value.Replace("\"", "");
            ValidateHex(value);
            bytes = DecodeHex(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Oid(byte[] value){
            bytes = new byte[12];
            Array.Copy(value,bytes,12);
        }


        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj){
            if(obj is Oid){
                return this.CompareTo((Oid)obj) == 0;
            }
            return false;
        }

        public bool Equals (Oid other){
            return this.CompareTo(other) == 0;
        }
        
        public int CompareTo (Oid other){
            if (System.Object.ReferenceEquals(other, null)){
                return 1;
            }
            byte[] otherBytes = other.ToByteArray();
            for(int x = 0; x < bytes.Length; x++){
                if(bytes[x] < otherBytes[x]){
                    return -1;
                }else if(bytes[x] > otherBytes[x]){
                    return 1;
                }
            }
            return 0;
        }
        
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            return String.Format("\"{0}\"",BitConverter.ToString(bytes).Replace("-","").ToLower());
        }
        
        /// <summary>
        /// Converts the Oid to a byte array. 
        /// </summary>
        public byte[] ToByteArray(){
            byte[] ret = new byte[12];
            Array.Copy(bytes, ret,12);
            return ret;
        }

        /// <summary>
        /// Generates an Oid using OidGenerator. 
        /// </summary>
        /// <returns>
        /// A <see cref="Oid"/>
        /// </returns>
        public static Oid NewOid(){
            return oidGenerator.Generate();   
        }

        public static bool operator ==(Oid a, Oid b){
            return a.Equals(b);
        }
    
        public static bool operator !=(Oid a, Oid b){
            return !(a == b);
        }
    
        public static bool operator >(Oid a, Oid b){
            return a.CompareTo(b) > 0;
        }
    
        public static bool operator <(Oid a, Oid b){
            return a.CompareTo(b) < 0;
        }
        
        
        /// <summary>
        /// Validates the hex.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void ValidateHex(string value){
            if(value == null || value.Length != 24) throw new ArgumentException("Oid strings should be 24 characters");
            
            Regex notHexChars = new Regex(@"[^A-Fa-f0-9]", RegexOptions.None);
            if(notHexChars.IsMatch(value)){
                throw new ArgumentOutOfRangeException("value","Value contains invalid characters");
            }
        }

        /// <summary>
        /// Decodes the hex.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected static byte[] DecodeHex(string value){
            int numberChars = value.Length;

            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2){
                try{
                    bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
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
