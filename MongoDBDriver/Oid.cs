using System;
using System.Text.RegularExpressions;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    /// <summary>
    ///   Oid is an immutable object that represents a Mongo ObjectId.
    /// </summary>
    public class Oid : IEquatable<Oid>, IComparable<Oid>
    {
        private static readonly OidGenerator OidGenerator = new OidGenerator();
        private readonly byte[] bytes;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Oid" /> class.
        /// </summary>
        /// <param name = "value">The value.</param>
        public Oid(string value)
        {
            value = value.Replace("\"", "");
            ValidateHex(value);
            bytes = DecodeHex(value);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Oid" /> class.
        /// </summary>
        /// <param name = "value">The value.</param>
        public Oid(byte[] value)
        {
            bytes = new byte[12];
            Array.Copy(value, bytes, 12);
        }

        /// <summary>
        ///   Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                var time = new byte[4];
                Array.Copy(bytes, time, 4);
                Array.Reverse(time);
                var seconds = BitConverter.ToInt32(time, 0);
                return BsonInfo.Epoch.AddSeconds(seconds);
            }
        }

        /// <summary>
        ///   Compares the current object with another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        ///   Value
        ///   Meaning
        ///   Less than zero
        ///   This object is less than the <paramref name = "other" /> parameter.
        ///   Zero
        ///   This object is equal to <paramref name = "other" />.
        ///   Greater than zero
        ///   This object is greater than <paramref name = "other" />.
        /// </returns>
        public int CompareTo(Oid other)
        {
            if(ReferenceEquals(other, null))
                return 1;
            var otherBytes = other.ToByteArray();
            for(var x = 0; x < bytes.Length; x++)
                if(bytes[x] < otherBytes[x])
                    return -1;
                else if(bytes[x] > otherBytes[x])
                    return 1;
            return 0;
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Oid other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if(obj is Oid)
                return CompareTo((Oid)obj) == 0;
            return false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("\"{0}\"", BitConverter.ToString(bytes).Replace("-", "").ToLower());
        }

        /// <summary>
        ///   Converts the Oid to a byte array.
        /// </summary>
        public byte[] ToByteArray()
        {
            var ret = new byte[12];
            Array.Copy(bytes, ret, 12);
            return ret;
        }

        /// <summary>
        ///   Generates an Oid using OidGenerator.
        /// </summary>
        /// <returns>
        ///   A <see cref = "Oid" />
        /// </returns>
        public static Oid NewOid()
        {
            return OidGenerator.Generate();
        }

        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Oid a, Oid b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Oid a, Oid b)
        {
            return !(a == b);
        }

        /// <summary>
        ///   Implements the operator &gt;.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Oid a, Oid b)
        {
            return a.CompareTo(b) > 0;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// <param name = "a">A.</param>
        /// <param name = "b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Oid a, Oid b)
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        ///   Validates the hex.
        /// </summary>
        /// <param name = "value">The value.</param>
        protected void ValidateHex(string value)
        {
            if(value == null || value.Length != 24)
                throw new ArgumentException("Oid strings should be 24 characters");

            var notHexChars = new Regex(@"[^A-Fa-f0-9]", RegexOptions.None);
            if(notHexChars.IsMatch(value))
                throw new ArgumentOutOfRangeException("value", "Value contains invalid characters");
        }

        /// <summary>
        ///   Decodes the hex.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        protected static byte[] DecodeHex(string value)
        {
            var numberChars = value.Length;

            var bytes = new byte[numberChars/2];
            for(var i = 0; i < numberChars; i += 2)
                try
                {
                    bytes[i/2] = Convert.ToByte(value.Substring(i, 2), 16);
                }
                catch
                {
                    //failed to convert these 2 chars, they may contain illegal charracters
                    bytes[i/2] = 0;
                }
            return bytes;
        }
    }
}