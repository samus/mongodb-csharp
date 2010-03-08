using System;
using System.Text.RegularExpressions;

namespace MongoDB.Driver{

    public class Oid
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public byte[] Value { get; set; }

        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created{
            get{
                byte[] time = new byte[4];
                Array.Copy(this.Value,time,4);
                Array.Reverse(time);
                int seconds = BitConverter.ToInt32(time,0);
                return OidGenerator.epoch.AddSeconds(seconds);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        public Oid(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Oid(string value){
            ValidateHex(value);
            this.Value = DecodeHex(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Oid(byte[] value){
            this.Value = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj){
            if(obj.GetType() == typeof(Oid)){
                string hex = obj.ToString();
                return this.ToString().Equals(hex);
            }
            return false;
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
            return String.Format("\"{0}\"",BitConverter.ToString(Value).Replace("-","").ToLower());
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
